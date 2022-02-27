using UnityEngine;
using static LevelConfigs;

public class ShipSpawner : MonoBehaviour
{
    private const float OFFSET_X = 12.5f;

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _parkingPoint;
    [SerializeField] private Transform _exitPoint;
    public Transform ParkingPoint => _parkingPoint;
    public Transform ExitPoint => _exitPoint;

    public Ship CreateShip(ShipConfig config)
    {
        var position = _spawnPoint.position;
        var boundX = config.ShipPrefab.ShipBody.sharedMesh.bounds.size.x;
        position.x = OFFSET_X + boundX * config.ShipPrefab.ShipBody.transform.localScale.x / 2;

        var ship = Instantiate(config.ShipPrefab, position, _spawnPoint.rotation).GetComponent<Ship>();
        ship.Init(config);
        return ship;
    }

}
