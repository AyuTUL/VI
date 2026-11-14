%option noyywrap nodefault

%{
#include <bits/stdc++.h>
using namespace std;

struct NFAState {
    map<char, vector<int>> go;
    vector<int> epsilon;
};

struct Fragment {
    int start;
    int accept;
};

static string regexText;
static bool processed = false;

static bool isSymbol(char c) {
    return isalnum(static_cast<unsigned char>(c));
}

static int precedence(char op) {
    if (op == '|') return 1;
    if (op == '.') return 2;
    return 0;
}

static string insertConcatenation(const string &re) {
    string result;
    for (size_t i = 0; i < re.size(); ++i) {
        char left = re[i];
        result += left;

        if (i + 1 == re.size()) break;
        char right = re[i + 1];

        bool canEnd = isSymbol(left) || left == ')' ||
                      left == '*' || left == '+' || left == '?';
        bool canStart = isSymbol(right) || right == '(';

        if (canEnd && canStart) result += '.';
    }
    return result;
}

static string toPostfix(const string &re) {
    string infix = insertConcatenation(re);
    string postfix;
    stack<char> operators;

    for (char c : infix) {
        if (isSymbol(c)) {
            postfix += c;
        } else if (c == '(') {
            operators.push(c);
        } else if (c == ')') {
            while (!operators.empty() && operators.top() != '(') {
                postfix += operators.top();
                operators.pop();
            }
            if (operators.empty())
                throw runtime_error("Unmatched parenthesis");
            operators.pop();
        } else if (c == '*' || c == '+' || c == '?') {
            postfix += c;
        } else if (c == '|' || c == '.') {
            while (!operators.empty() && operators.top() != '(' &&
                   precedence(operators.top()) >= precedence(c)) {
                postfix += operators.top();
                operators.pop();
            }
            operators.push(c);
        } else {
            throw runtime_error(string("Unsupported symbol: ") + c);
        }
    }

    while (!operators.empty()) {
        if (operators.top() == '(')
            throw runtime_error("Unmatched parenthesis");
        postfix += operators.top();
        operators.pop();
    }
    return postfix;
}

static int addState(vector<NFAState> &nfa) {
    nfa.push_back(NFAState{});
    return static_cast<int>(nfa.size()) - 1;
}

static Fragment buildNFA(const string &postfix,
                         vector<NFAState> &nfa,
                         set<char> &alphabet) {
    stack<Fragment> fragments;

    for (char c : postfix) {
        if (isSymbol(c)) {
            int start = addState(nfa);
            int accept = addState(nfa);
            nfa[start].go[c].push_back(accept);
            alphabet.insert(c);
            fragments.push({start, accept});
        } else if (c == '.') {
            if (fragments.size() < 2)
                throw runtime_error("Invalid concatenation");
            Fragment second = fragments.top(); fragments.pop();
            Fragment first = fragments.top(); fragments.pop();
            nfa[first.accept].epsilon.push_back(second.start);
            fragments.push({first.start, second.accept});
        } else if (c == '|') {
            if (fragments.size() < 2)
                throw runtime_error("Invalid union");
            Fragment second = fragments.top(); fragments.pop();
            Fragment first = fragments.top(); fragments.pop();
            int start = addState(nfa);
            int accept = addState(nfa);
            nfa[start].epsilon.push_back(first.start);
            nfa[start].epsilon.push_back(second.start);
            nfa[first.accept].epsilon.push_back(accept);
            nfa[second.accept].epsilon.push_back(accept);
            fragments.push({start, accept});
        } else if (c == '*') {
            if (fragments.empty())
                throw runtime_error("Invalid Kleene star");
            Fragment old = fragments.top(); fragments.pop();
            int start = addState(nfa);
            int accept = addState(nfa);
            nfa[start].epsilon.push_back(old.start);
            nfa[start].epsilon.push_back(accept);
            nfa[old.accept].epsilon.push_back(old.start);
            nfa[old.accept].epsilon.push_back(accept);
            fragments.push({start, accept});
        } else if (c == '+') {
            if (fragments.empty())
                throw runtime_error("Invalid plus closure");
            Fragment old = fragments.top(); fragments.pop();
            int start = addState(nfa);
            int accept = addState(nfa);
            nfa[start].epsilon.push_back(old.start);
            nfa[old.accept].epsilon.push_back(old.start);
            nfa[old.accept].epsilon.push_back(accept);
            fragments.push({start, accept});
        } else if (c == '?') {
            if (fragments.empty())
                throw runtime_error("Invalid optional operator");
            Fragment old = fragments.top(); fragments.pop();
            int start = addState(nfa);
            int accept = addState(nfa);
            nfa[start].epsilon.push_back(old.start);
            nfa[start].epsilon.push_back(accept);
            nfa[old.accept].epsilon.push_back(accept);
            fragments.push({start, accept});
        }
    }

    if (fragments.size() != 1)
        throw runtime_error("Malformed regular expression");
    return fragments.top();
}

static vector<int> epsilonClosure(const vector<NFAState> &nfa,
                                  const vector<int> &states) {
    set<int> closure(states.begin(), states.end());
    stack<int> pending;
    for (int state : states) pending.push(state);

    while (!pending.empty()) {
        int current = pending.top();
        pending.pop();
        for (int next : nfa[current].epsilon) {
            if (closure.insert(next).second)
                pending.push(next);
        }
    }
    return vector<int>(closure.begin(), closure.end());
}

static vector<int> moveAndClose(const vector<NFAState> &nfa,
                                const vector<int> &states,
                                char symbol) {
    set<int> moved;
    for (int state : states) {
        auto found = nfa[state].go.find(symbol);
        if (found != nfa[state].go.end())
            moved.insert(found->second.begin(), found->second.end());
    }
    vector<int> result(moved.begin(), moved.end());
    return epsilonClosure(nfa, result);
}

static string stateSetText(const vector<int> &states) {
    string result = "{";
    for (size_t i = 0; i < states.size(); ++i) {
        if (i != 0) result += ',';
        result += to_string(states[i]);
    }
    return result + "}";
}

static bool convertRegexToDFA(const string &re) {
    try {
        string postfix = toPostfix(re);
        vector<NFAState> nfa;
        set<char> alphabetSet;
        Fragment complete = buildNFA(postfix, nfa, alphabetSet);

        vector<char> alphabet(alphabetSet.begin(), alphabetSet.end());
        vector<vector<int>> dfaStates;
        vector<vector<int>> transition;
        map<vector<int>, int> stateNumber;
        queue<int> unprocessed;

        vector<int> startSet = epsilonClosure(nfa, {complete.start});
        dfaStates.push_back(startSet);
        transition.push_back(vector<int>(alphabet.size(), -1));
        stateNumber[startSet] = 0;
        unprocessed.push(0);

        while (!unprocessed.empty()) {
            int current = unprocessed.front();
            unprocessed.pop();

            for (size_t column = 0; column < alphabet.size(); ++column) {
                vector<int> next = moveAndClose(
                    nfa, dfaStates[current], alphabet[column]);

                if (next.empty()) continue;

                if (!stateNumber.count(next)) {
                    int newNumber = static_cast<int>(dfaStates.size());
                    stateNumber[next] = newNumber;
                    dfaStates.push_back(next);
                    transition.push_back(vector<int>(alphabet.size(), -1));
                    unprocessed.push(newNumber);
                }
                transition[current][column] = stateNumber[next];
            }
        }

        cout << "\nPostfix expression: " << postfix << '\n';
        cout << "DFA start state: D0\n";
        cout << "DFA final state(s): ";
        for (size_t i = 0; i < dfaStates.size(); ++i) {
            if (find(dfaStates[i].begin(), dfaStates[i].end(), complete.accept)
                != dfaStates[i].end()) {
                cout << 'D' << i << ' ';
            }
        }

        cout << "\n\nDFA state subsets:\n";
        for (size_t i = 0; i < dfaStates.size(); ++i)
            cout << 'D' << i << " = " << stateSetText(dfaStates[i]) << '\n';

        cout << "\nTransition table\nState";
        for (char symbol : alphabet) cout << '\t' << symbol;
        cout << '\n';

        for (size_t state = 0; state < dfaStates.size(); ++state) {
            cout << 'D' << state;
            for (int target : transition[state]) {
                if (target == -1) cout << "\t-";
                else cout << "\tD" << target;
            }
            cout << '\n';
        }
        return true;
    } catch (const exception &error) {
        cerr << "Error: " << error.what() << '\n';
        return false;
    }
}
%}

%%

[a-zA-Z0-9|*+?()]   { regexText.append(yytext, yyleng); }
[ \t\r]+             { /* Ignore spaces. */ }
\n                   {
                        if (regexText.empty()) {
                            cerr << "Error: empty regular expression\n";
                            return 1;
                        }
                        processed = true;
                        return convertRegexToDFA(regexText) ? 0 : 1;
                     }
.                    {
                        cerr << "Invalid character in RE: " << yytext << '\n';
                        return 1;
                     }
<<EOF>>              {
                        if (!processed && !regexText.empty())
                            return convertRegexToDFA(regexText) ? 0 : 1;
                        return 0;
                     }

%%

int main() {
    cout << "Enter regular expression: ";
    return yylex();
}
