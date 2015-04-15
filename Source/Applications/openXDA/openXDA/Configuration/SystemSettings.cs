﻿//******************************************************************************************************
//  SystemSettings.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/30/2014 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using GSF;
using GSF.Configuration;

namespace openXDA.Configuration
{
    /// <summary>
    /// Represents the system settings for openXDA.
    /// </summary>
    public class SystemSettings
    {
        #region [ Members ]

        // Fields
        private string m_dbConnectionString;

        private string m_watchDirectories;
        private string m_resultsPath;
        private string m_filePattern;

        private string m_timeZone;
        private double m_timeTolerance;

        private double m_maxVoltage;
        private double m_maxCurrent;
        private double m_lowVoltageThreshold;
        private double m_maxLowVoltageCurrent;
        private double m_maxTimeOffset;
        private double m_minTimeOffset;

        private double m_residualCurrentTrigger;
        private double m_phaseCurrentTrigger;
        private double m_prefaultTrigger;
        private double m_faultSuppressionTrigger;
        private double m_maxFaultDistanceMultiplier;
        private double m_minFaultDistanceMultiplier;

        private string m_lengthUnits;
        private double m_comtradeMinWaitTime;
        private int m_processingThreadCount;
        private string m_fileShares;

        private string m_smtpServer;
        private string m_fromAddress;

        private string m_pqDashboardUrl;

        private List<string> m_watchDirectoryList;
        private List<FileShare> m_fileShareList;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="SystemSettings"/> class.
        /// </summary>
        public SystemSettings()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SystemSettings"/> class.
        /// </summary>
        /// <param name="connectionString">A string containing the system settings as key-value pairs.</param>
        public SystemSettings(string connectionString)
        {
            ConnectionStringParser<SettingAttribute> parser = new ConnectionStringParser<SettingAttribute>();
            parser.ParseConnectionString(connectionString, this);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the connection string to the database.
        /// </summary>
        [Setting]
        [DefaultValue("Data Source=localhost; Initial Catalog=openXDA; Integrated Security=SSPI")]
        public string DbConnectionString
        {
            get
            {
                return m_dbConnectionString;
            }
            set
            {
                m_dbConnectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of directories to watch for files.
        /// </summary>
        [Setting]
        [DefaultValue("Watch")]
        public string WatchDirectories
        {
            get
            {
                return m_watchDirectories;
            }
            set
            {
                m_watchDirectories = value;

                if ((object)value != null)
                    m_watchDirectoryList = value.Split(Path.PathSeparator).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the path to the fault location
        /// results generated by the fault location engine.
        /// </summary>
        [Setting]
        [DefaultValue("Results")]
        public string ResultsPath
        {
            get
            {
                return m_resultsPath;
            }
            set
            {
                m_resultsPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the pattern used to parse file paths in
        /// order to identify the meter that the file came from.
        /// </summary>
        [Setting]
        [DefaultValue(@"(?<AssetKey>[^\\]+)\\[^\\]+$")]
        public string FilePattern
        {
            get
            {
                return m_filePattern;
            }
            set
            {
                m_filePattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the time zone identifier for the
        /// time zone used by all meters in the system.
        /// </summary>
        [Setting]
        [DefaultValue("UTC")]
        public string TimeZone
        {
            get
            {
                return m_timeZone;
            }
            set
            {
                m_timeZone = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum distance, in seconds,
        /// between a meter's clock and real time.
        /// </summary>
        [Setting]
        [DefaultValue(0.5D)]
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

        /// <summary>
        /// Gets or sets the per-unit threshold at which the
        /// voltage exceeds engineering reasonableness.
        /// </summary>
        [Setting]
        [DefaultValue(2.0D)]
        public double MaxVoltage
        {
            get
            {
                return m_maxVoltage;
            }
            set
            {
                m_maxVoltage = value;
            }
        }

        /// <summary>
        /// Gets or sets the per-unit threshold at which the
        /// current exceeds engineering reasonableness.
        /// </summary>
        [Setting]
        [DefaultValue(8.0D)]
        public double MaxCurrent
        {
            get
            {
                return m_maxCurrent;
            }
            set
            {
                m_maxCurrent = value;
            }
        }

        /// <summary>
        /// Gets or sets the per-unit threshold at which the
        /// voltage is classified as a low voltage.
        /// </summary>
        [Setting]
        [DefaultValue(0.5D)]
        public double LowVoltageThreshold
        {
            get
            {
                return m_lowVoltageThreshold;
            }
            set
            {
                m_lowVoltageThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the per-unit threshold at which the current
        /// exceeds engineering reasonableness when the voltage is low.
        /// </summary>
        [Setting]
        [DefaultValue(1.0D)]
        public double MaxLowVoltageCurrent
        {
            get
            {
                return m_maxLowVoltageCurrent;
            }
            set
            {
                m_maxLowVoltageCurrent = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of hours beyond the current system time
        /// before the time of the record indicates that the data is unreasonable.
        /// </summary>
        [Setting]
        [DefaultValue(24.0D)]
        public double MaxTimeOffset
        {
            get
            {
                return m_maxTimeOffset;
            }
            set
            {
                m_maxTimeOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of hours prior to the current system time
        /// before the time of the record indicates that the data is unreasonable.
        /// </summary>
        [Setting]
        [DefaultValue(1440.0D)]
        public double MinTimeOffset
        {
            get
            {
                return m_minTimeOffset;
            }
            set
            {
                m_minTimeOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the per-unit threshold at which the
        /// residual current indicates faulted conditions.
        /// </summary>
        [Setting]
        [DefaultValue(0.5D)]
        public double ResidualCurrentTrigger
        {
            get
            {
                return m_residualCurrentTrigger;
            }
            set
            {
                m_residualCurrentTrigger = value;
            }
        }

        /// <summary>
        /// Gets or sets the per-unit threshold at which the
        /// phase currents indicate faulted conditions.
        /// </summary>
        [Setting]
        [DefaultValue(4.0D)]
        public double PhaseCurrentTrigger
        {
            get
            {
                return m_phaseCurrentTrigger;
            }
            set
            {
                m_phaseCurrentTrigger = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold at which the ratio between RMS
        /// current and prefault RMS current indicates faulted conditions.
        /// </summary>
        [Setting]
        [DefaultValue(5.0D)]
        public double PrefaultTrigger
        {
            get
            {
                return m_prefaultTrigger;
            }
            set
            {
                m_prefaultTrigger = value;
            }
        }

        /// <summary>
        /// Gets or sets the per-unit threshold under which the current indicates
        /// non-faulted conditions even if the current exceeds the prefault trigger.
        /// </summary>
        [Setting]
        [DefaultValue(1.5D)]
        public double FaultSuppressionTrigger
        {
            get
            {
                return m_faultSuppressionTrigger;
            }
            set
            {
                m_faultSuppressionTrigger = value;
            }
        }

        /// <summary>
        /// Gets or sets the multiplier applied to the line length to determine
        /// the maximum value allowed for fault distance in the COMTRADE export.
        /// </summary>
        [Setting]
        [DefaultValue(1.05D)]
        public double MaxFaultDistanceMultiplier
        {
            get
            {
                return m_maxFaultDistanceMultiplier;
            }
            set
            {
                m_maxFaultDistanceMultiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets the multiplier applied to the line length to determine
        /// the minimum value allowed for fault distance in the COMTRADE export.
        /// </summary>
        [Setting]
        [DefaultValue(-0.05D)]
        public double MinFaultDistanceMultiplier
        {
            get
            {
                return m_minFaultDistanceMultiplier;
            }
            set
            {
                m_minFaultDistanceMultiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets the units of measure to use
        /// for lengths (line length and fault distance).
        /// </summary>
        [Setting]
        [DefaultValue("miles")]
        public string LengthUnits
        {
            get
            {
                return m_lengthUnits;
            }
            set
            {
                m_lengthUnits = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum amount of time to wait for additional data
        /// files after the system detects the existence of a .D00 COMTRADE file.
        /// </summary>
        [Setting]
        [DefaultValue(15.0D)]
        public double COMTRADEMinWaitTime
        {
            get
            {
                return m_comtradeMinWaitTime;
            }
            set
            {
                m_comtradeMinWaitTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of threads used
        /// for processing meter data concurrently.
        /// </summary>
        /// <remarks>
        /// Values less than or equal to zero will be set to the number of logical processors.
        /// </remarks>
        [Setting]
        [DefaultValue(0)]
        public int ProcessingThreadCount
        {
            get
            {
                return m_processingThreadCount;
            }
            set
            {
                m_processingThreadCount = value;

                if (m_processingThreadCount <= 0)
                    m_processingThreadCount = Environment.ProcessorCount;
            }
        }

        /// <summary>
        /// Gets or sets a list of parameters used
        /// to authenticate to multiple file shares.
        /// </summary>
        [Setting]
        [DefaultValue("")]
        public string FileShares
        {
            get
            {
                return m_fileShares;
            }
            set
            {
                m_fileShares = value;

                if ((object)value != null)
                {
                    m_fileShareList = value.ParseKeyValuePairs()
                        .Select(kvp => kvp.Value)
                        .Select(fileShareString => new FileShare(fileShareString))
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the hostname or IP address of the SMTP server to
        /// use for sending automated email notifications when faults occur.
        /// </summary>
        [Setting]
        [DefaultValue("")]
        public string SMTPServer
        {
            get
            {
                return m_smtpServer;
            }
            set
            {
                m_smtpServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the email address used when sending automated email notifications.
        /// </summary>
        [Setting]
        [DefaultValue("openXDA@gridprotectionalliance.org")]
        public string FromAddress
        {
            get
            {
                return m_fromAddress;
            }
            set
            {
                m_fromAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the URL of the PQ Dashboard.
        /// </summary>
        [Setting]
        [DefaultValue("http://pqdashboard/")]
        public string PQDashboardURL
        {
            get
            {
                return m_pqDashboardUrl;
            }
            set
            {
                m_pqDashboardUrl = value;
            }
        }

        /// <summary>
        /// Gets a list of directories to be watched
        /// for files containing fault records.
        /// </summary>
        public IReadOnlyCollection<string> WatchDirectoryList
        {
            get
            {
                return m_watchDirectoryList.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a list of file shares to be authenticated at startup.
        /// </summary>
        public IReadOnlyCollection<FileShare> FileShareList
        {
            get
            {
                return m_fileShareList.AsReadOnly();
            }
        }

        #endregion

        #region [ Methods ]

        public string ToConnectionString()
        {
            ConnectionStringParser<SettingAttribute> parser = new ConnectionStringParser<SettingAttribute>();
            parser.ExplicitlySpecifyDefaults = true;
            return parser.ComposeConnectionString(this);
        }

        #endregion
    }
}
