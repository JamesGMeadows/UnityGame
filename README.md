# UnityGame
Unity client / server code for a game I'm working on in my free time

# Network Implementation
Uses TCP to send and recieve packets. The first 2 bytes define the packet type. Packets have a predefined length.
So a position packet will have the structure (2, 4, 4, 4) = 14 where the first 2 bytes are the packet declaration and the last 12
bytes are floats of 4 bytes each. Each of the 3 floats corresponds to a position in 3D space (x,y,z).

Once all 14 bytes are read, the listener knows it can start on the next packet.


# Functionality
The client server currently sends position + rotation updates as well as damage and entity status updates such as opening doors.
