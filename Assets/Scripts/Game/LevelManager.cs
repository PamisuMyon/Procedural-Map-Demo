using UnityEngine;

[System.Obsolete]
public class LevelManager : MonoBehaviour 
{

    public static LevelManager Instance { get; private set; }

    private void Awake() 
    {
        Instance = this;
    }

    private void Start() 
    {
        PlacePlayer();
    }

    void PlacePlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        var gates = GameObject.FindGameObjectsWithTag("Gate");
        var spawIndex = Random.Range(0, gates.Length);
        var gate = gates[spawIndex];
        foreach (var player in players)
        {
            var forward = Vector3.ProjectOnPlane(gate.transform.forward, player.transform.up);
            player.transform.forward = forward.normalized;
            player.transform.position = gate.transform.position;
        }
        Destroy(gate);
    }

}