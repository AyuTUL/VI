%option noyywrap nodefault

%{
#include <bits/stdc++.h>
using namespace std;

static vector<string> inputTokens;
%}

%%

-?[0-9]+            { inputTokens.emplace_back(yytext, yyleng); }
[^ \t\r\n]           { inputTokens.emplace_back(yytext, yyleng); }
[ \t\r\n]+           { /* Ignore whitespace. */ }

%%

static size_t tokenPosition = 0;

static string nextToken(const string &fieldName) {
    if (tokenPosition >= inputTokens.size())
        throw runtime_error("Missing value for " + fieldName);
    return inputTokens[tokenPosition++];
}

static int nextInteger(const string &fieldName) {
    string token = nextToken(fieldName);
    size_t used = 0;
    int value;
    try {
        value = stoi(token, &used);
    } catch (...) {
        throw runtime_error("Expected integer for " + fieldName +
                            ", found '" + token + "'");
    }
    if (used != token.size())
        throw runtime_error("Expected integer for " + fieldName +
                            ", found '" + token + "'");
    return value;
}

int main() {
    cout << "Enter DFA in this order:\n"
         << "n m\n"
         << "m alphabet symbols\n"
         << "start_state\n"
         << "number_of_final_states final_state_list\n"
         << "n x m transition table\n\n";

    yylex();

    try {
        int stateCount = nextInteger("number of states");
        int symbolCount = nextInteger("number of symbols");
        if (stateCount <= 0 || symbolCount <= 0)
            throw runtime_error("State and symbol counts must be positive");

        vector<char> alphabet(symbolCount);
        for (int i = 0; i < symbolCount; ++i) {
            string symbol = nextToken("alphabet symbol");
            if (symbol.size() != 1)
                throw runtime_error("Every alphabet symbol must be one character");
            alphabet[i] = symbol[0];
        }

        int start = nextInteger("start state");
        if (start < 0 || start >= stateCount)
            throw runtime_error("Invalid start state");

        int finalCount = nextInteger("number of final states");
        if (finalCount < 0 || finalCount > stateCount)
            throw runtime_error("Invalid number of final states");

        vector<bool> isFinal(stateCount, false);
        for (int i = 0; i < finalCount; ++i) {
            int state = nextInteger("final state");
            if (state < 0 || state >= stateCount)
                throw runtime_error("Invalid final state");
            isFinal[state] = true;
        }

        vector<vector<int>> transition(
            stateCount, vector<int>(symbolCount));

        for (int state = 0; state < stateCount; ++state) {
            for (int symbol = 0; symbol < symbolCount; ++symbol) {
                int target = nextInteger("transition target");
                if (target < 0 || target >= stateCount)
                    throw runtime_error("Invalid transition target");
                transition[state][symbol] = target;
            }
        }

        if (tokenPosition != inputTokens.size())
            throw runtime_error("Extra input found after transition table");

        vector<bool> reachable(stateCount, false);
        queue<int> pending;
        reachable[start] = true;
        pending.push(start);

        while (!pending.empty()) {
            int current = pending.front();
            pending.pop();
            for (int target : transition[current]) {
                if (!reachable[target]) {
                    reachable[target] = true;
                    pending.push(target);
                }
            }
        }

        vector<int> nonFinalStates;
        vector<int> finalStates;
        for (int state = 0; state < stateCount; ++state) {
            if (!reachable[state]) continue;
            if (isFinal[state]) finalStates.push_back(state);
            else nonFinalStates.push_back(state);
        }

        vector<vector<int>> partitions;
        if (!nonFinalStates.empty()) partitions.push_back(nonFinalStates);
        if (!finalStates.empty()) partitions.push_back(finalStates);

        bool changed = true;
        while (changed) {
            changed = false;

            vector<int> groupOf(stateCount, -1);
            for (size_t group = 0; group < partitions.size(); ++group)
                for (int state : partitions[group])
                    groupOf[state] = static_cast<int>(group);

            vector<vector<int>> refined;
            for (const vector<int> &partition : partitions) {
                map<vector<int>, vector<int>> sameSignature;

                for (int state : partition) {
                    vector<int> signature;
                    for (int symbol = 0; symbol < symbolCount; ++symbol)
                        signature.push_back(
                            groupOf[transition[state][symbol]]);
                    sameSignature[signature].push_back(state);
                }

                if (sameSignature.size() > 1) changed = true;
                for (auto &entry : sameSignature)
                    refined.push_back(entry.second);
            }
            partitions = refined;
        }

        vector<int> groupOf(stateCount, -1);
        for (size_t group = 0; group < partitions.size(); ++group)
            for (int state : partitions[group])
                groupOf[state] = static_cast<int>(group);

        cout << "\nEquivalent-state groups:\n";
        for (size_t group = 0; group < partitions.size(); ++group) {
            cout << 'M' << group << " = {";
            for (size_t i = 0; i < partitions[group].size(); ++i) {
                if (i != 0) cout << ',';
                cout << partitions[group][i];
            }
            cout << "}\n";
        }

        cout << "Minimized start state: M" << groupOf[start] << '\n';
        cout << "Minimized final state(s): ";
        for (size_t group = 0; group < partitions.size(); ++group) {
            if (isFinal[partitions[group][0]])
                cout << 'M' << group << ' ';
        }

        cout << "\n\nMinimized transition table\nState";
        for (char symbol : alphabet) cout << '\t' << symbol;
        cout << '\n';

        for (size_t group = 0; group < partitions.size(); ++group) {
            int representative = partitions[group][0];
            cout << 'M' << group;
            for (int symbol = 0; symbol < symbolCount; ++symbol)
                cout << "\tM"
                     << groupOf[transition[representative][symbol]];
            cout << '\n';
        }

        cout << "\nUnreachable states removed: ";
        bool foundUnreachable = false;
        for (int state = 0; state < stateCount; ++state) {
            if (!reachable[state]) {
                cout << state << ' ';
                foundUnreachable = true;
            }
        }
        if (!foundUnreachable) cout << "none";
        cout << '\n';
    } catch (const exception &error) {
        cerr << "Input error: " << error.what() << '\n';
        return 1;
    }

    return 0;
}
