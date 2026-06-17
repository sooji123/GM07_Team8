using UnityEngine;

public class PathManager : Singleton<PathManager>
{
    [Header("WayPoint List")]
    [SerializeField] private Transform[] _wayPoints;
    public int WayPointCount { get => _wayPoints.Length; }
    //에너미가 현 목표지점을 가져올 때 사용(특정 인덱스 웨이포인트 반환)
    public Transform GetWayPoint(int index)
    {
        if (index < 0 || index >= _wayPoints.Length)
        {
            return null;
        }
        return _wayPoints[index];
    }
}
