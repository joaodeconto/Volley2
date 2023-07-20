using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public static class NetworkObjectExtensions
{
    public static async Task<bool> WaitForStateAuthority(this NetworkObject o, float maxWaitTime = 8f)
    {
        float waitStartTime = Time.time;
        o.RequestStateAuthority();

        while (!o.HasStateAuthority && (Time.time - waitStartTime) < maxWaitTime)
        {
            await Task.Delay(1);
        }

        return o.HasStateAuthority;
    }
}
