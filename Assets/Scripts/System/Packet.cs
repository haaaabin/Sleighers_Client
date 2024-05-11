using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packet
{
    public string email;
}

public class UserInfo : Packet
{
    public string name;
    public string cart;
    public int curCostume;
    public int curCart;
}

// 매치메이킹 요청 패킷
public class IdPacket
{
    public string id;
}

public class MatchInfo : IdPacket
{
    public string name;
    public int curCart;
}