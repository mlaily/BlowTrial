﻿using BlowTrial.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.CustomSorters
{
    class ParticipantNameSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.Name.CompareTo(partY.Name);
        }
    }
    class ParticipantIdSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.Id.CompareTo(partY.Id);
        }
    }
    class ParticipantRegisteredAtSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.RegisteredAt.CompareTo(partY.RegisteredAt);
        }
    }
    class ParticipantDateTimeBirthSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.DateTimeBirth.CompareTo(partY.DateTimeBirth);
        }
    }
    class ParticipantHospitalIdSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.HospitalIdentifier.CompareTo(partY.HospitalIdentifier);
        }
    }
    class ParticipantDataRequiredSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.DataRequired.CompareTo(partY.DataRequired);
        }
    }
    class ParticipantMarkedFinishedSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return partX.UserMarkedFinished.CompareTo(partY.UserMarkedFinished);
        }
    }

    class ParticipantMarkedFinishedSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.UserMarkedFinished.CompareTo(partY.UserMarkedFinished);
        }
    }

    class ParticipantNameSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.Name.CompareTo(partY.Name);
        }
    }
    class ParticipantIdSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.Id.CompareTo(partY.Id);
        }
    }
    class ParticipantRegisteredAtSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.RegisteredAt.CompareTo(partY.RegisteredAt);
        }
    }
    class ParticipantDateTimeBirthSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.DateTimeBirth.CompareTo(partY.DateTimeBirth);
        }
    }
    class ParticipantHospitalIdSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.HospitalIdentifier.CompareTo(partY.HospitalIdentifier);
        }
    }
    class ParticipantDataRequiredSortDesc : IComparer
    {
        public int Compare(object x, object y)
        {
            var partX = (ParticipantListItemViewModel)x;
            var partY = (ParticipantListItemViewModel)y;
            return -partX.DataRequired.CompareTo(partY.DataRequired);
        }
    }
}
