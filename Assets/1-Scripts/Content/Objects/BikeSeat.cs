using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame {
    
    public sealed class BikeSeat : Seat {
        
        protected override List<SittingPosition> defaultSittingPositions {
            get {
                return new List<SittingPosition>() {
                    new Seat.SittingPosition(new Vector3(1f, -1.2f, -0.8f), Quaternion.LookRotation(Vector3.left, Vector3.up), 1),
                    new Seat.SittingPosition(new Vector3(-1f, -1.2f, -0.8f), Quaternion.LookRotation(Vector3.right, Vector3.up), 2)
                };
            }
        }

    }
}