using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : INetworkSerializable
{
    public FixedString32Bytes PlayerName;
    public TeamID TeamID;
    public PlayerClass PlayerClass;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref TeamID);
        serializer.SerializeValue(ref PlayerClass);
    }

    public override string ToString()
    {
        return $"PlayerData(Name: {PlayerName}, Team: {TeamID}, Class: {PlayerClass})";
    }
}