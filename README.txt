The configuration file is dadstorm.config, it should be located alongside the puppet master's binaries, but can be altered in puppet master's app.config file.
Logs are output to Log.txt in the same directory as puppet master. Again, it is editable in the app.config file.


To start the project, either open up DADSTORM.sln in visual studio or run the included binaries directly.
1 - PCSs must be run first, and will require no configuration (each PCS will bind to a new port).
2 - Then the Puppet master must be executed.
3 - Commands are input directly in the command line if not from the config file.


As for missing features, the custom operator sometimes loads correclty, while others it says that the binary is invalid.
-Hash routing is not working, routing was simplified to one route, then that node replicating to others after sending the tuples to the next operator.
-Log file is called asynchronously, resulting in tuples appearing in odd orders.
-The Custom operator does not always load dlls correctly when passed to the "Assembly.Load(code)".
-Starting operations that require other nodes will not work, when data is sent to a node, it starts executing immediately. (freezing works though)
-Due to the way operations were implemented, the interval function was changed to be interval between config file script executions.
-PCS clearing up existing nodes was not implemented.
-Fault tolerance was not implemented.

Sometimes, during execution, the PCS process will lose information regarding which nodes represent a certain operation (i.e. OP1) and be unable to reach them. This does not always happen, and watching the variable didn't yield an explaination.
