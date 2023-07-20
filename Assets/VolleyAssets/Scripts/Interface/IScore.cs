using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScore 
{
    void AddPoint(int team);
    void Result(int team);
    void ClearScore();
}
