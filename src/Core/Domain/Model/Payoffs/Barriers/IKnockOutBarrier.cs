using System;
using System.Collections.Generic;
using System.Text;

namespace Domain {
    public interface IKnockOutBarrier {
        IReadOnlyList<DateTime> ObservationDates { get; }
        MonitoringFrequency MonitoringFrequency { get; }
        bool IsTriggered(IPricePath path, DateTime observationDate);
        double GetRedemption(IPricePath path, DateTime triggerDate);
    }
}
