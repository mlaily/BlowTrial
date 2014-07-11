﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BlowTrial.Infrastructure.Interfaces
{
    public class DispatcherTimerAdaper : DispatcherTimer, IDispatcherTimer 
    { 
        public DispatcherTimerAdaper() : base()
        { }
        public DispatcherTimerAdaper(DispatcherPriority priority) : base(priority)
        { }
    }
    public class MoqTimerEventArgs : EventArgs
    {
        public MoqTimerEventArgs() : this(DateTime.Now)
        { }
        public MoqTimerEventArgs(DateTime startAt)
        {
            StartAt = startAt;
        }
        DateTime _startSetAt;
        DateTime _startAt;
        public DateTime StartAt 
        {
            get 
            {
                return _startAt;
            } 
            set
            {
                _startSetAt = DateTime.Now;
                _startAt = value;
            }
        }
        public DateTime LastNowCall {get; set;}
        public DateTime Now
        {
            get
            {
                LastNowCall = DateTime.Now;
                return _startAt + (LastNowCall - _startSetAt);
            }
        }
    }
    public interface IDispatcherTimer
    {
        TimeSpan Interval { get; set; }
        bool IsEnabled { get; set; }
        object Tag { get; set; }
        event EventHandler Tick;
        void Start();
        void Stop();
    }
}
