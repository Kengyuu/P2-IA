using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    [RequireComponent(typeof(FSM_RouteExecutor))]
    [RequireComponent(typeof(ROOMBA_Blackboard))]
    public class FSM1_DUSTSEARCH : FiniteStateMachine
    {

        public enum State { INITIAL, PATROLLING, GO_TO_DUST, DUST_REACHED };
        public State currentState = State.INITIAL; 
        private ROOMBA_Blackboard blackboard;
        private FSM_RouteExecutor patrolling;

        public GameObject dust;
        public GameObject nearestDust; 

        void Awake()
        {
            blackboard = GetComponent<ROOMBA_Blackboard>();
            patrolling = GetComponent<FSM_RouteExecutor>(); 
        }

        public override void Exit()
        {
            patrolling.Exit(); 
        }

        public override void ReEnter()
        {
            base.ReEnter();
            currentState = State.INITIAL;
        }
       
        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.PATROLLING);
                    break;
                case State.PATROLLING:
                    dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.closeDustDetectionRadius);
                    if (dust != null)
                    {
                        ChangeState(State.GO_TO_DUST);
                        break;
                    }
                    else 
                    {
                        dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.farDustDetectionRadius); 
                        if(dust!= null)
                        {
                            Debug.Log("break");
                            ChangeState(State.GO_TO_DUST);
                            break; 
                        }
                    }
                    if(patrolling.currentState == FSM_RouteExecutor.State.TERMINATED)
                    {
                        ChangeState(State.PATROLLING);
                        break; 
                    }
                    break;
                case State.GO_TO_DUST:
                    nearestDust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.closeDustDetectionRadius);
                    if(nearestDust != null && nearestDust != dust)
                    {
                        dust = nearestDust;
                        ChangeState(State.GO_TO_DUST);
                        break; 
                    }
                    if (SensingUtils.DistanceToTarget(gameObject,dust) < blackboard.dustReachedRadius)
                    {
                        ChangeState(State.DUST_REACHED);
                        break; 
                    }
                    break;
                case State.DUST_REACHED:
                    ChangeState(State.PATROLLING); 
                    break; 
            } 
        }



        private void ChangeState(State newState)
        {

         
            switch (currentState)
            {
                case State.PATROLLING:
                    patrolling.Exit(); 
                    break;
                case State.GO_TO_DUST:
                    patrolling.Exit(); 
                    break;
                case State.DUST_REACHED:
                    break;
            }

         
            switch (newState)
            {
                case State.PATROLLING:
                    patrolling.ReEnter();
                    patrolling.target = blackboard.GetRandomWanderPoint(); 
                    break;
                case State.GO_TO_DUST:
                    patrolling.ReEnter();
                    patrolling.target = dust;
                    break;
                case State.DUST_REACHED:
                    dust.gameObject.SetActive(false); 
                    break;
            } 
            currentState = newState;

        } 


       
    }
}
