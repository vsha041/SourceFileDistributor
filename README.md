# SourceFileDistributor
Optimally distributes the file of a particular type into minimum number of folders.

# Purpose of this tool
Lot of times we are in a situation when our code on the Dev machine is referencing some libraries that were built on build servers and as a result the PDB files built on those build servers contain hardcoded file paths pointing to build server's hard disk locations. You can use the dumpbin command to find this info by the way. As a result even if you have the correct source code, and if you try to debug any of those third party assembly, you will get a pop-up window that looks like this http://i.stack.imgur.com/LV1jn.png. If you use this tool then you are never going to see that window again. In an enterprise application we will have hundreds of DLL like this and as a result debugging experience will be very bad. A lot of time will be wasted telling VS Debugger about each source code file one by one.

This tool solves that problem by optimally distributing the source files of a repository into minimum number of folders.
