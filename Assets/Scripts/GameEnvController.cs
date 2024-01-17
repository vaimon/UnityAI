using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActivator : MonoBehaviour
{
    public UnityEvent onActivate { get; } = new UnityEvent();
    public UnityEvent onDeactivate { get; } = new UnityEvent();
}

public class GameEnvController : MonoBehaviour
{
    public int buttonsOnEpisode = 4;
    public int boxesOnEpisode = 3;
    public int agentsOnEpisode = 1;

    private SimpleMultiAgentGroup agents;
    private ButtonActivator[] activators;
    private int activeCounter = 0;


    public GridedDistributor buttonsDistributor;
    public GridedDistributor boxDistributor;
    public GridedDistributor agentsDistributor;
    public MeshCollider goal;

    void Start()
    {
        ResetScene();
    }

    void ResetScene()
    {
        var buttons = buttonsDistributor.Respawn(buttonsOnEpisode);
        boxDistributor.Respawn(boxesOnEpisode);

        var activators = new ButtonActivator[buttons.Length];
        for (var i = 0; i < buttons.Length; i++)
            activators[i] = buttons[i].GetComponent<Button>();
        ResetActivators(activators);

        if (agents != null)
        {
            foreach (var agent in agents.GetRegisteredAgents().ToArray())
            {
                (agent as AgentPusher).onMaxStepsReached.RemoveListener(onMaxStepsReached);
                agents.UnregisterAgent(agent);
            }
        }
        agents = new SimpleMultiAgentGroup();
       
        foreach (var agentObject in agentsDistributor.Respawn(agentsOnEpisode))
        {
            var agent = agentObject.GetComponent<Agent>();
            agents.RegisterAgent(agent);
            (agent as AgentPusher).onMaxStepsReached.AddListener(onMaxStepsReached);
        }
    }

    public void OnGoalTriggered()
    {
        agents.SetGroupReward(1f);
        agents.EndGroupEpisode();
        ResetScene();
    }

    private void onMaxStepsReached()
    {
        Debug.Log("Max steps reached. Restarting...");
        agents.GroupEpisodeInterrupted();
        ResetScene();
    }


    private void OnActivate()
    {
        agents.AddGroupReward(0.5f);
        activeCounter++;
        if (activeCounter == activators.Length)
            OnGoalTriggered();
    }
    private void onDeactivate()
    {
        agents.AddGroupReward(-0.5f);
        activeCounter--;
    }
    public void ResetActivators(ButtonActivator[] newActivators)
    {
        if (activators != null)
            foreach (var activator in activators)
            {
                activator.onActivate.RemoveListener(OnActivate);
                activator.onDeactivate.RemoveListener(onDeactivate);
            }
        activators = newActivators;
        gameObject.SetActive(true);
        foreach (var activator in activators)
        {
            activator.onActivate.AddListener(OnActivate);
            activator.onDeactivate.AddListener(onDeactivate);
        }
    }
}
