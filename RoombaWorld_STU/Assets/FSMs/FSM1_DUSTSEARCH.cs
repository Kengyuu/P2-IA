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

        public enum State { INITIAL, PATROLLING, GO_TO_DUST, GO_TO_POO };
        public State currentState = State.INITIAL; 
        private ROOMBA_Blackboard blackboard;
        private FSM_RouteExecutor patrolling;

        public GameObject dust;
        public GameObject nearestDust;
        public GameObject poo;
       

        void Start()
        {
            blackboard = GetComponent<ROOMBA_Blackboard>();
            patrolling = GetComponent<FSM_RouteExecutor>();
            //patrolling.Exit(); 
        }

        public override void Exit()
        {
            patrolling.Exit();
            base.Exit(); 
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
                   
                    //POO PART 
                    poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.closePooDetectionRadius);
                    if (poo == null)
                        poo = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "POO", blackboard.farPooDetectionRadius);

                    if (poo != null)
                    {
                        ChangeState(State.GO_TO_POO);
                        break;
                    }
                    // DUST PART
                    dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.closeDustDetectionRadius);
                    if(dust == null) 
                        dust = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "DUST", blackboard.farDustDetectionRadius);

                    if (dust != null)
                    {
                        ChangeState(State.GO_TO_DUST);
                        break;
                    }                   
                    if(patrolling.currentState == FSM_RouteExecutor.State.TERMINATED)
                    {
                        ChangeState(State.PATROLLING);
                        break; 
                    }
                    
                    break;

                case State.GO_TO_DUST:

                    poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.closePooDetectionRadius);
                    if (poo == null)
                        poo = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "POO", blackboard.farPooDetectionRadius);

                    if (poo != null)
                    {
                        ChangeState(State.GO_TO_POO);
                        break;
                    }

                    nearestDust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.closeDustDetectionRadius);
                    if(nearestDust != null && nearestDust != dust)
                    {
                        dust = nearestDust;
                        ChangeState(State.GO_TO_DUST);
                        break; 
                    }
                    if (SensingUtils.DistanceToTarget(gameObject,dust) < blackboard.dustReachedRadius)
                    {
                        dust.gameObject.SetActive(false);
                        ChangeState(State.PATROLLING);
                        break; 
                    }
                    break;
                case State.GO_TO_POO:
                    dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.closeDustDetectionRadius);
                    if(dust != null && !blackboard.memory.Contains(dust))
                    {
                        blackboard.AddToMemory(dust); 
                    }
                    GameObject nearestPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.closePooDetectionRadius);
                    if(nearestPoo != poo && nearestPoo != null)
                    {
                        poo = nearestPoo;
                        ChangeState(State.GO_TO_POO);
                        break; 
                    }
                    if (SensingUtils.DistanceToTarget(gameObject, poo) < blackboard.pooReachedRadius)
                    {
                        poo.gameObject.SetActive(false);
                        if (blackboard.memory.Count > 0)
                        {
                            dust = blackboard.RetrieveFromMemory();
                            ChangeState(State.GO_TO_DUST);
                            break;
                        }
                        ChangeState(State.PATROLLING);
                        break;
                    }
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
                case State.GO_TO_POO:
                    patrolling.Exit(); 
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
                case State.GO_TO_POO:
                    patrolling.ReEnter();
                    patrolling.target = poo; 
                    break; 
               
            } 
            currentState = newState;

        } 


       
    }
}
