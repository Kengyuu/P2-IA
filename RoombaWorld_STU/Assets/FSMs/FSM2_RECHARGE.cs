using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    
    public class FSM2_RECHARGE : FiniteStateMachine {

        public enum State { INITIAL, CLEANING, GO_TO_STATION,RECHARGING };
        public State currentState = State.INITIAL;
        private ROOMBA_Blackboard blackboard;
        private FSM1_DUSTSEARCH dustSearcher;
        private FSM_RouteExecutor goRecharge;

        GameObject energyStation; 

        void Start() {
            blackboard = GetComponent<ROOMBA_Blackboard>();
            dustSearcher = GetComponent<FSM1_DUSTSEARCH>();
            goRecharge = GetComponent<FSM_RouteExecutor>();
        }

        public override void Exit() {
            dustSearcher.Exit();
            goRecharge.Exit();
            base.Exit(); 
        }

        public override void ReEnter() {
            base.ReEnter();
            currentState = State.INITIAL;
        }

        void Update() {
            switch (currentState) {
                case State.INITIAL:
                    ChangeState(State.CLEANING);
                    break;
                case State.CLEANING:
                    if (blackboard.currentCharge <= blackboard.minCharge) {
                        energyStation = blackboard.GetClosestEnergyStation(this.gameObject); 
                        ChangeState(State.GO_TO_STATION);
                        break;
                    }
                    break;
                case State.GO_TO_STATION:
                    if(SensingUtils.DistanceToTarget(gameObject,energyStation) < blackboard.chargingStationReachedRadius)
                        ChangeState(State.RECHARGING);
                    break;
                case State.RECHARGING:
                    blackboard.Recharge(Time.deltaTime);
                    if(blackboard.currentCharge >= 100)
                    {
                        ChangeState(State.CLEANING);
                        break; 
                    }
                    break;
            }
        }


        private void ChangeState(State newState) {
            switch (currentState) {
                case State.CLEANING:
                    dustSearcher.Exit(); 
                    break;
                case State.GO_TO_STATION:
                    goRecharge.Exit(); 
                    break; 
                case State.RECHARGING:
                    blackboard.currentCharge = blackboard.maxCharge;
                    break;
            }

            switch (newState) {
                case State.CLEANING:
                    dustSearcher.ReEnter(); 
                    break;
                case State.GO_TO_STATION:
                    goRecharge.ReEnter();
                    goRecharge.target = energyStation; 
                    break;
                case State.RECHARGING:
                    break;
            }
            currentState = newState;
        }
    }
}
