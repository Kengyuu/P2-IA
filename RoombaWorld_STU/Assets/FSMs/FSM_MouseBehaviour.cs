using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    [RequireComponent(typeof(FSM_RouteExecutor))]
    public class FSM_MouseBehaviour : FiniteStateMachine {

        public enum State { INITIAL, MOVE, FLEE};
        public State currentState = State.INITIAL; 
        
        MOUSE_Blackboard blackboard;
        FSM_RouteExecutor movingTowardsPoint;
        SpriteRenderer spr;
        KinematicState ks;

        public GameObject point;

        void Start() {
            blackboard = GetComponent<MOUSE_Blackboard>();
            movingTowardsPoint = GetComponent<FSM_RouteExecutor>();
            spr = GetComponent<SpriteRenderer>();
            ks = GetComponent<KinematicState>();
        }

        public override void Exit() {
            base.Exit(); 
        }

        public override void ReEnter() {
            base.ReEnter();
            currentState = State.INITIAL;
        }
       
        void Update() {
            switch (currentState) {
                case State.INITIAL:
                    ChangeState(State.MOVE);
                    break;
                case State.MOVE:
                    if(SensingUtils.DistanceToTarget(gameObject, movingTowardsPoint.target) < blackboard.pointReachedRadius){
                        if(blackboard.hasPooped){
                            Debug.Log("Destroyed");
                            Destroy(gameObject);
                        }
                        else{
                            blackboard.hasPooped = true;
                            Instantiate(blackboard.pooPrefab, transform.position, Quaternion.identity);
                            point = blackboard.RandomExitPoint();
                            ChangeState(State.MOVE);
                        }
                    }
                    if(SensingUtils.FindInstanceWithinRadius(gameObject, "ROOMBA", blackboard.roombaDetectionRadius)){
                        point = blackboard.NearestExitPoint();
                        ChangeState(State.FLEE);
                        spr.color = blackboard.afraidColor;
                        blackboard.hasPooped = true;
                    }
                    break;
                case State.FLEE:
                    if(SensingUtils.DistanceToTarget(gameObject, movingTowardsPoint.target) < blackboard.pointReachedRadius)
                        Destroy(gameObject);
                break;
            } 
        }



        private void ChangeState(State newState) {         
            switch (currentState) {
                case State.MOVE:
                    movingTowardsPoint.Exit();
                    break;               
                case State.FLEE:
                    movingTowardsPoint.Exit();
                    break;               
            }

            switch (newState) {
                case State.MOVE:
                    movingTowardsPoint.ReEnter();
                    movingTowardsPoint.target = point;
                    break;
                case State.FLEE:
                    movingTowardsPoint.ReEnter();
                    ks.maxSpeed = ks.maxSpeed * 2;
                    movingTowardsPoint.target = point;
                    break;
               
            } 
            currentState = newState;

        } 


       
    }
}
