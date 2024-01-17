using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class GameEnvController : MonoBehaviour
{
    public int buttonsOnEpisode = 4;
    public int boxesOnEpisode = 3;
    public int agentsOnEpisode = 1;

    private Agent agent;
    public GridedDistributor buttonsDistributor;
    public GridedDistributor boxDistributor;
    public GridedDistributor agentsDistributor;
    public Door door;
    public MeshCollider goal;

    void Start()
    {
        ResetScene();
    }

    void ResetScene()
    {
        var buttons = buttonsDistributor.Respawn(buttonsOnEpisode);
        boxDistributor.Respawn(boxesOnEpisode);
        var activators = new DoorActivator[buttons.Length];
        for (var i = 0; i < buttons.Length; i++)
            activators[i] = buttons[i].GetComponent<Button>();
        door.ResetActivators(activators);

        agent = agentsDistributor.Respawn(agentsOnEpisode)[0].GetComponent<Agent>();
    }

    public void OnGoalTriggered()
    {
        agent.SetReward(1f);
        agent.EndEpisode();
        ResetScene();
    }
    void FixedUpdate()
    {
    }
}
