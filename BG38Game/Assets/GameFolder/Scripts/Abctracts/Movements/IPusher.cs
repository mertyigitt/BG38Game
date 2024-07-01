using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG38Game.Abstracts.Movements
{
    public interface IPusher
    {
        public bool IsPushing { get; set; }
        void PushAction();
    }
}
