using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    // struct to hold all networked player data

    public ulong clientId;
    public int colourId;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colourId == other.colourId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colourId);
    }
}
