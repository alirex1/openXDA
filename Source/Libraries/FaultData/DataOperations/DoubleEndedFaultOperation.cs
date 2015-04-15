﻿//******************************************************************************************************
//  DoubleEndedFaultOperation.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  04/07/2015 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using FaultAlgorithms;
using FaultData.DataAnalysis;
using FaultData.Database;
using FaultData.DataResources;
using FaultData.DataSets;
using GSF;

namespace FaultData.DataOperations
{
    public class DoubleEndedFaultOperation : DataOperationBase<MeterDataSet>, IDisposable
    {
        #region [ Members ]

        // Nested Types
        private class FaultTimeline
        {
            public Meter Meter;
            public List<FaultLocationData.FaultSummaryRow> Faults;
        }

        private class Mapping
        {
            public MappingNode Left;
            public MappingNode Right;

            public Mapping(FaultLocationData.FaultSummaryRow left, FaultLocationData.FaultSummaryRow right)
            {
                Left = new MappingNode(left);
                Right = new MappingNode(right);
            }
        }

        private class MappingNode
        {
            #region [ Members ]

            // Constants
            // TODO: Hardcoded frequency
            public const double Frequency = 60.0D;

            // Fields
            public FaultLocationData.FaultSummaryRow Fault;
            public VICycleDataGroup CycleDataGroup;
            public Fault.Curve DistanceCurve;
            public Fault.Curve AngleCurve;

            public FaultType FaultType;
            public int StartSample;
            public int EndSample;

            #endregion

            #region [ Constructors ]

            public MappingNode(FaultLocationData.FaultSummaryRow fault)
            {
                Fault = fault;
                DistanceCurve = new Fault.Curve("DoubleEnded");
                AngleCurve = new Fault.Curve("DoubleEnded");

                if (!Enum.TryParse(fault.FaultType, out FaultType))
                    FaultType = FaultType.None;
            }

            #endregion

            #region [ Methods ]

            public void Initialize(DbAdapterContainer dbAdapterContainer, VICycleDataGroup viCycleDataGroup)
            {
                int samplesPerCycle = (int)Math.Round(viCycleDataGroup.VA.RMS.SampleRate / Frequency);

                FaultSegment faultSegment = dbAdapterContainer.FaultLocationInfoAdapter.FaultSegments
                    .Where(segment => segment.EventID == Fault.EventID)
                    .Where(segment => segment.StartTime == Fault.Inception)
                    .FirstOrDefault(segment => segment.SegmentType.Name == "Fault");

                if ((object)faultSegment != null)
                {
                    StartSample = faultSegment.StartSample;
                    EndSample = faultSegment.EndSample - samplesPerCycle + 1;
                    CycleDataGroup = Rotate(viCycleDataGroup.ToSubSet(StartSample, EndSample));

                    DistanceCurve.StartIndex = StartSample;
                    AngleCurve.StartIndex = StartSample;
                }
            }

            private VICycleDataGroup Rotate(VICycleDataGroup viCycleDataGroup)
            {
                DataSeries referenceSeries = viCycleDataGroup.VA.Phase;

                DataGroup shiftedDataGroup = new DataGroup(new List<DataSeries>()
                {
                    viCycleDataGroup.VA.RMS,
                    Rotate(viCycleDataGroup.VA.Phase, referenceSeries),
                    viCycleDataGroup.VA.Peak,
                    viCycleDataGroup.VA.Error,

                    viCycleDataGroup.VB.RMS,
                    Rotate(viCycleDataGroup.VB.Phase, referenceSeries),
                    viCycleDataGroup.VB.Peak,
                    viCycleDataGroup.VB.Error,

                    viCycleDataGroup.VC.RMS,
                    Rotate(viCycleDataGroup.VC.Phase, referenceSeries),
                    viCycleDataGroup.VC.Peak,
                    viCycleDataGroup.VC.Error,

                    viCycleDataGroup.IA.RMS,
                    Rotate(viCycleDataGroup.IA.Phase, referenceSeries),
                    viCycleDataGroup.IA.Peak,
                    viCycleDataGroup.IA.Error,

                    viCycleDataGroup.IB.RMS,
                    Rotate(viCycleDataGroup.IB.Phase, referenceSeries),
                    viCycleDataGroup.IB.Peak,
                    viCycleDataGroup.IB.Error,

                    viCycleDataGroup.IC.RMS,
                    Rotate(viCycleDataGroup.IC.Phase, referenceSeries),
                    viCycleDataGroup.IC.Peak,
                    viCycleDataGroup.IC.Error,

                    viCycleDataGroup.IR.RMS,
                    Rotate(viCycleDataGroup.IR.Phase, referenceSeries),
                    viCycleDataGroup.IR.Peak,
                    viCycleDataGroup.IR.Error
                });

                return new VICycleDataGroup(shiftedDataGroup);
            }

            private DataSeries Rotate(DataSeries phaseSeries, DataSeries referenceSeries)
            {
                Func<DataPoint, DataPoint, DataPoint> shiftFunc;
                DataSeries shiftedPhaseSeries;

                shiftFunc = (phase, reference) => new DataPoint()
                {
                    Time = phase.Time,
                    Value = phase.Value - reference.Value
                };

                shiftedPhaseSeries = new DataSeries();

                shiftedPhaseSeries.DataPoints = phaseSeries.DataPoints
                    .Zip(referenceSeries.DataPoints, shiftFunc)
                    .ToList();

                return shiftedPhaseSeries;
            }

            #endregion
        }

        // Fields
        private DbAdapterContainer m_dbAdapterContainer;
        private TimeZoneInfo m_timeZone;
        private double m_timeTolerance;

        private List<MappingNode> m_processedMappingNodes;
        private FaultLocationData.FaultCurveDataTable m_faultCurveTable;
        private FaultLocationData.DoubleEndedFaultDistanceDataTable m_doubleEndedFaultDistanceTable;

        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public DoubleEndedFaultOperation()
        {
            m_processedMappingNodes = new List<MappingNode>();
            m_faultCurveTable = new FaultLocationData.FaultCurveDataTable();
            m_doubleEndedFaultDistanceTable = new FaultLocationData.DoubleEndedFaultDistanceDataTable();
        }

        #endregion

        #region [ Properties ]

        public string TimeZone
        {
            get
            {
                return m_timeZone.Id;
            }
            set
            {
                m_timeZone = TimeZoneInfo.FindSystemTimeZoneById(value);
            }
        }

        public double TimeTolerance
        {
            get
            {
                return m_timeTolerance;
            }
            set
            {
                m_timeTolerance = value;
            }
        }

        #endregion

        #region [ Methods ]

        public override void Prepare(DbAdapterContainer dbAdapterContainer)
        {
            m_dbAdapterContainer = dbAdapterContainer;
        }

        public override void Execute(MeterDataSet meterDataSet)
        {
            CycleDataResource cycleDataResource;
            FaultDataResource faultDataResource;

            List<Tuple<DateTime, DateTime>> eventTimeRanges;
            MeterData.EventDataTable systemEvent;

            double lineLength;
            List<Mapping> mappings;

            int leftEventID;
            int rightEventID;
            VICycleDataGroup leftCycleDataGroup;
            VICycleDataGroup rightCycleDataGroup;

            cycleDataResource = meterDataSet.GetResource<CycleDataResource>();
            faultDataResource = meterDataSet.GetResource(() => new FaultDataResource(m_dbAdapterContainer));

            // Get a time range for querying each system event that contains events in this meter data set
            eventTimeRanges = GetEventTimeRanges(cycleDataResource.DataGroups.Where(dataGroup => faultDataResource.FaultLookup.ContainsKey(dataGroup)));

            foreach (Tuple<DateTime, DateTime> timeRange in eventTimeRanges)
            {
                // Get the full collection of events from the database that comprise the system event that overlaps this time range
                systemEvent = m_dbAdapterContainer.EventAdapter.GetSystemEvent(timeRange.Item1, timeRange.Item2, m_timeTolerance);

                foreach (IGrouping<int, MeterData.EventRow> lineGrouping in systemEvent.GroupBy(evt => evt.LineID))
                {
                    // Make sure this line connects two known meter locations
                    if (m_dbAdapterContainer.MeterInfoAdapter.MeterLocationLines.Count(mll => mll.LineID == lineGrouping.Key) != 2)
                        continue;

                    // Determine the length of the line
                    lineLength = m_dbAdapterContainer.MeterInfoAdapter.Lines
                        .Where(line => line.ID == lineGrouping.Key)
                        .Select(line => (double?)line.Length)
                        .FirstOrDefault() ?? double.NaN;

                    if (double.IsNaN(lineLength))
                        continue;

                    leftEventID = 0;
                    rightEventID = 0;
                    leftCycleDataGroup = null;
                    rightCycleDataGroup = null;

                    // Attempt to match faults during this system event that occurred
                    // on one end of the line with faults that occurred during this
                    // system even on the other end of the line
                    mappings = GetMappings(lineGrouping);

                    foreach (Mapping mapping in mappings)
                    {
                        if (mapping.Left.FaultType == FaultType.None || mapping.Right.FaultType == FaultType.None)
                            continue;

                        // Get the cycle data for each of the two mapped faults
                        if (mapping.Left.Fault.EventID != leftEventID)
                        {
                            leftEventID = mapping.Left.Fault.EventID;
                            leftCycleDataGroup = GetCycleData(leftEventID);
                        }

                        if (mapping.Right.Fault.EventID != rightEventID)
                        {
                            rightEventID = mapping.Right.Fault.EventID;
                            rightCycleDataGroup = GetCycleData(rightEventID);
                        }

                        if ((object)leftCycleDataGroup == null || (object)rightCycleDataGroup == null)
                            continue;

                        // Make sure there are no other threads calculating double-ended fault distance for this mapping,
                        // and that double-ended distance has not already been calculated and entered into the database
                        lock (FaultSummaryIDLock)
                        {
                            if (FaultSummaryIDs.Contains(mapping.Left.Fault.ID))
                                continue;

                            if (FaultSummaryIDs.Contains(mapping.Right.Fault.ID))
                                continue;

                            if (m_dbAdapterContainer.DoubleEndedFaultDistanceAdapter.GetCountBy(mapping.Left.Fault.ID) > 0)
                                continue;

                            if (m_dbAdapterContainer.DoubleEndedFaultDistanceAdapter.GetCountBy(mapping.Right.Fault.ID) > 0)
                                continue;

                            FaultSummaryIDs.Add(mapping.Left.Fault.ID);
                            FaultSummaryIDs.Add(mapping.Right.Fault.ID);
                        }

                        // Initialize the mappings with additional data needed for double-ended fault location
                        mapping.Left.Initialize(m_dbAdapterContainer, leftCycleDataGroup);
                        mapping.Right.Initialize(m_dbAdapterContainer, rightCycleDataGroup);

                        // Execute the double-ended fault location algorithm
                        ExecuteFaultLocationAlgorithm(lineLength, mapping.Left, mapping.Right);
                        ExecuteFaultLocationAlgorithm(lineLength, mapping.Right, mapping.Left);

                        // Create rows in the DoubleEndedFaultDistance table
                        CreateFaultDistanceRow(mapping.Left, mapping.Right);
                        CreateFaultDistanceRow(mapping.Right, mapping.Left);

                        // Add these nodes to the collection of processed mapping nodes
                        m_processedMappingNodes.Add(mapping.Left);
                        m_processedMappingNodes.Add(mapping.Right);
                    }
                }
            }

            // Create a row in the FaultCurve table for every event that now has double-ended fault distance curves
            foreach (IGrouping<int, MappingNode> grouping in m_processedMappingNodes.GroupBy(node => node.Fault.EventID))
                CreateFaultCurveRow(grouping);
        }

        public override void Load(DbAdapterContainer dbAdapterContainer)
        {
            BulkLoader loader = new BulkLoader();
            loader.Connection = dbAdapterContainer.Connection;
            loader.Load(m_doubleEndedFaultDistanceTable);
            loader.Load(m_faultCurveTable);
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    lock (FaultSummaryIDLock)
                    {
                        foreach (MappingNode node in m_processedMappingNodes)
                            FaultSummaryIDs.Remove(node.Fault.ID);
                    }
                }
                finally
                {
                    m_disposed = true;
                }
            }
        }

        private List<Tuple<DateTime, DateTime>> GetEventTimeRanges(IEnumerable<DataGroup> dataGroups)
        {
            return dataGroups
                .OrderBy(dataGroup => dataGroup.StartTime)
                .Select(dataGroup => new
                {
                    StartTime = TimeZoneInfo.ConvertTimeToUtc(dataGroup.StartTime, m_timeZone).AddSeconds(-m_timeTolerance),
                    EndTime = TimeZoneInfo.ConvertTimeToUtc(dataGroup.EndTime, m_timeZone).AddSeconds(m_timeTolerance),
                })
                .Aggregate(new List<Tuple<DateTime, DateTime>>(), (list, timeRange) =>
                {
                    Tuple<DateTime, DateTime> last = list.Last();
                    DateTime lastStartTime = last.Item1;
                    DateTime lastEndTime = last.Item2;

                    DateTime minStartTime;
                    DateTime maxEndTime;

                    bool overlap;

                    // Determine if last and timeRange overlap
                    overlap = (lastStartTime <= timeRange.StartTime && timeRange.EndTime <= lastEndTime) ||
                              (timeRange.StartTime <= lastStartTime && lastStartTime <= timeRange.EndTime);

                    if (!overlap)
                    {
                        // Add timeRange as new tuple
                        list.Add(Tuple.Create(timeRange.StartTime, timeRange.EndTime));
                    }
                    else
                    {
                        // Merge last and timeRange
                        minStartTime = Common.Min(lastStartTime, timeRange.StartTime);
                        maxEndTime = Common.Max(lastEndTime, timeRange.EndTime);
                        list[list.Count - 1] = Tuple.Create(minStartTime, maxEndTime);
                    }

                    return list;
                });
        }

        private List<Mapping> GetMappings(IGrouping<int, MeterData.EventRow> lineGrouping)
        {
            List<FaultTimeline> meterGroupings = lineGrouping
                .GroupBy(evt => evt.MeterID)
                .Select(meterGrouping => new FaultTimeline()
                {
                    Meter = m_dbAdapterContainer.MeterInfoAdapter.Meters.SingleOrDefault(meter => meter.ID == meterGrouping.Key),
                    Faults = meterGrouping.SelectMany(evt => m_dbAdapterContainer.FaultSummaryAdapter.GetDataBy(evt.ID)).Where(fault => fault.IsSelectedAlgorithm != 0).OrderBy(fault => fault.Inception).ToList()
                })
                .Where(meterGrouping => meterGrouping.Faults.Any())
                .ToList();

            return meterGroupings
                .SelectMany(meterGrouping1 => meterGroupings.Select(meterGrouping2 => new
                {
                    Left = meterGrouping1,
                    Right = meterGrouping2
                }))
                .Where(mapping => mapping.Left.Meter.MeterLocationID < mapping.Right.Meter.MeterLocationID)
                .Where(mapping => mapping.Left.Faults.Count == mapping.Right.Faults.Count)
                .SelectMany(mapping => mapping.Left.Faults.Zip(mapping.Right.Faults, (left, right) => new Mapping(left, right)))
                .ToList();
        }

        private void CreateFaultCurveRow(IGrouping<int, MappingNode> grouping)
        {
            VICycleDataGroup viCycleDataGroup = GetCycleData(grouping.Key);
            DataGroup faultCurveGroup = new DataGroup();

            faultCurveGroup.Add(viCycleDataGroup.VA.RMS.Multiply(double.NaN));
            faultCurveGroup.Add(faultCurveGroup[0].Copy());

            foreach (MappingNode node in grouping)
            {
                for (int i = node.StartSample; node.DistanceCurve.HasData(i); i++)
                {
                    faultCurveGroup[0][i].Value = node.DistanceCurve[i].Value;
                    faultCurveGroup[1][i].Value = node.AngleCurve[i].Value;
                }
            }

            m_faultCurveTable.AddFaultCurveRow(grouping.Key, "DoubleEnded", faultCurveGroup.ToData());
        }

        private void CreateFaultDistanceRow(MappingNode local, MappingNode remote)
        {
            FaultLocationData.DoubleEndedFaultDistanceRow row;

            row = m_doubleEndedFaultDistanceTable.NewDoubleEndedFaultDistanceRow();
            row.LocalFaultSummaryID = local.Fault.ID;
            row.RemoteFaultSummaryID = remote.Fault.ID;
            row.Distance = local.DistanceCurve[local.Fault.CalculationCycle].Value;
            row.Angle = local.AngleCurve[local.Fault.CalculationCycle].Value;

            m_doubleEndedFaultDistanceTable.AddDoubleEndedFaultDistanceRow(row);
        }

        private void ExecuteFaultLocationAlgorithm(double lineLength, MappingNode local, MappingNode remote)
        {
            FaultLocationDataSet faultLocationDataSet;
            CycleData remoteCycle;
            ComplexNumber[] curve;

            faultLocationDataSet = new FaultLocationDataSet();
            faultLocationDataSet.FaultType = local.FaultType;
            faultLocationDataSet.LineDistance = lineLength;
            local.CycleDataGroup.PushDataTo(faultLocationDataSet.Cycles);

            remoteCycle = GetCycleAt(remote.CycleDataGroup, remote.Fault.CalculationCycle - remote.StartSample);

            curve = FaultLocationAlgorithms.DoubleEnded(faultLocationDataSet, remoteCycle, string.Empty);
            ExtractDistanceVectors(local, curve);
        }

        private void ExtractDistanceVectors(MappingNode local, ComplexNumber[] curve)
        {
            List<DataPoint> vaRMS = local.CycleDataGroup.VA.RMS.DataPoints;
            int count = Math.Min(vaRMS.Count, curve.Length);

            for (int i = 0; i < count; i++)
            {
                local.DistanceCurve.Series.DataPoints.Add(new DataPoint()
                {
                    Time = vaRMS[i].Time,
                    Value = curve[i].Magnitude
                });

                local.AngleCurve.Series.DataPoints.Add(new DataPoint()
                {
                    Time = vaRMS[i].Time,
                    Value = curve[i].Angle
                });
            }
        }

        private VICycleDataGroup GetCycleData(int eventID)
        {
            MeterData.CycleDataDataTable cycleDataTable;
            DataGroup dataGroup;

            cycleDataTable = m_dbAdapterContainer.CycleDataAdapter.GetDataBy(eventID);

            if (cycleDataTable.Count == 0)
                return null;

            dataGroup = new DataGroup();
            dataGroup.FromData(cycleDataTable[0].Data);

            return new VICycleDataGroup(dataGroup);
        }

        private CycleData GetCycleAt(VICycleDataGroup viCycleDataGroup, int index)
        {
            CycleData cycle = new CycleData();

            Cycle[] cycles =
            {
                cycle.AN.V,
                cycle.BN.V,
                cycle.CN.V,
                cycle.AN.I,
                cycle.BN.I,
                cycle.CN.I
            };

            CycleDataGroup[] cycleDataGroups =
            {
                viCycleDataGroup.VA,
                viCycleDataGroup.VB,
                viCycleDataGroup.VC,
                viCycleDataGroup.IA,
                viCycleDataGroup.IB,
                viCycleDataGroup.IC
            };

            for (int i = 0; i < cycles.Length; i++)
            {
                cycles[i].RMS = cycleDataGroups[i].RMS[index].Value;
                cycles[i].Phase = cycleDataGroups[i].Phase[index].Value;
                cycles[i].Peak = cycleDataGroups[i].Peak[index].Value;
                cycles[i].Error = cycleDataGroups[i].Error[index].Value;
            }

            return cycle;
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly HashSet<int> FaultSummaryIDs = new HashSet<int>();
        private static readonly object FaultSummaryIDLock = new object();

        #endregion
    }
}
