using UnityEngine;

namespace Wormhole
{
    public interface IPortalSpace
    {
        void Clear();

        bool HasPortal(int portal);

        int AddPortal(string namePrefix, Vector3 n, Vector3 p, object args);

        bool RemovePortal(int portal);

        int LinkPortals(int portalIn, int portalOut);

        void UnlinkPortals(int link);
    }
}
