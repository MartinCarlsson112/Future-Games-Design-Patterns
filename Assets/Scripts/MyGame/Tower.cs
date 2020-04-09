using UnityEngine;

public class Tower : MonoBehaviour
{
    BulletType bulletType;

    public void UpdateTower(TowerManager towerManager)
    {
        //check if enemy is nearby

        //if nearby, SHOOT!
            //towerManager.RequestShoot(transform.position, new Vector3(100, 10, 100), BulletType.Normal);

        //else idle
    }

    private void OnDestroy()
    {
        //send event, which tower manager listens to
    }
}
