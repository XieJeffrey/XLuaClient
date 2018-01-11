
c:/protobuf-2.6.1/src/protoc.exe --descriptor_set_out=Hall.pb --proto_path="./" --include_imports Hall.proto

c:/protobuf-net/protogen.exe -i:Hall.proto -o:../../Proto/Hall_pb.cs

c:/protobuf-2.6.1/src/protoc.exe --descriptor_set_out=Game.pb --proto_path="./" --include_imports Game.proto

c:/protobuf-net/protogen.exe -i:Game.proto -o:../../Proto/Game_pb.cs

c:/protobuf-2.6.1/src/protoc.exe --descriptor_set_out=Guild.pb --proto_path="./" --include_imports Guild.proto

c:/protobuf-net/protogen.exe -i:Guild.proto -o:../../Proto/Guild_pb.cs

c:/protobuf-2.6.1/src/protoc.exe --descriptor_set_out=Entity.pb --proto_path="./" --include_imports Entity.proto

c:/protobuf-net/protogen.exe -i:Entity.proto -o:../../Proto/Entity_pb.cs


pause