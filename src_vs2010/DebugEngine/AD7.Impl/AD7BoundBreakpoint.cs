﻿//* Copyright 2010-2011 Research In Motion Limited.
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//* http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License.

using System;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlackBerry.DebugEngine
{

    /// <summary> 
    /// This class represents a breakpoint that has been bound to a location in the debuggee. It is a child of the pending 
    /// breakpoint that creates it. Unless the pending breakpoint only has one bound breakpoint, each bound breakpoint is displayed as 
    /// a child of the pending breakpoint in the breakpoints window. Otherwise, only one is displayed. 
    /// (http://msdn.microsoft.com/en-us/library/bb161979.aspx)
    /// </summary>
    public sealed class AD7BoundBreakpoint : IDebugBoundBreakpoint2
    {
        private readonly AD7PendingBreakpoint _pendingBreakpoint;
        private readonly AD7BreakpointResolution _breakpointResolution;
        private readonly AD7Engine _engine;

        private bool _enabled;
        private bool _deleted;
        public uint _hitCount;

        public uint m_bpLocationType;
        public string m_filename = "";
        public string m_fullPath = "";
        public uint m_line = 0;
        public string m_func = "";

        /// <summary> 
        /// This breakpoint's index in the list of active bound breakpoints. 
        /// </summary>
        private int m_remoteID = -1;
        public int RemoteID
        {
            get { return m_remoteID; }
        }

        public BP_PASSCOUNT m_bpPassCount;
        public BP_CONDITION m_bpCondition;

        /// <summary>
        /// TRUE if the program has to stop when the hit count is equal to a given value.
        /// </summary>
        public bool _isHitCountEqual;

        /// <summary>
        /// Different than 0 if the program has to stop whenever the hit count is multiple of a given value
        /// </summary>
        public uint _hitCountMultiple; 
        public bool _breakWhenCondChanged;
        public string _previousCondEvaluation = "";
        
        /// <summary>
        /// Indicates if a given breakpoint is being manipulated in one of these 2 methods: SetPassCount and BreakpointHit.
        /// </summary>
        public bool _blockedPassCount; 

        /// <summary>
        /// Indicates if a given breakpoint is being manipulated in one of these 2 methods: SetCondition and BreakpointHit.
        /// </summary>
        public bool _blockedConditional;

        /// <summary> 
        /// GDB member variables. 
        /// </summary>
        private uint _GDB_ID;
        private string _GDB_filename = "";
        private uint _GDB_linePos;
        private string _GDB_Address = "";


        /// <summary> 
        /// GDB_ID Property. 
        /// </summary>
        public uint GDB_ID
        {
            get { return _GDB_ID; }
            set { _GDB_ID = value; }
        }

        /// <summary> 
        /// GDB_FileName Property. 
        /// </summary>
        public string GDB_FileName
        {
            get { return _GDB_filename; }
            set { _GDB_filename = value; }
        }

        
        /// <summary> 
        /// GDB_LinePos Property. 
        /// </summary>
        public uint GDB_LinePos
        {
            get { return _GDB_linePos; }
            set { _GDB_linePos = value; }
        }

        
        /// <summary> 
        /// GDB_Address Property. 
        /// </summary>
        public string GDB_Address
        {
            get { return _GDB_Address; }
            set { _GDB_Address = value; }
        }


        /// <summary> 
        /// GDB works with short path names only, which requires converting the path names to/from long ones. This function 
        /// returns the short path name for a given long one. 
        /// </summary>
        /// <param name="path">Long path name. </param>
        /// <param name="shortPath">Returns this short path name. </param>
        /// <param name="shortPathLength"> Lenght of this short path name. </param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
                 [MarshalAs(UnmanagedType.LPTStr)]
                   string path,
                 [MarshalAs(UnmanagedType.LPTStr)]
                   StringBuilder shortPath,
                 int shortPathLength
                 );


        /// <summary> 
        /// AD7BoundBreakpoint constructor for file/line breaks. 
        /// </summary>
        /// <param name="engine"> AD7 Engine. </param>
        /// <param name="bpReqInfo"> Contains the information required to implement a breakpoint. </param>
        /// <param name="pendingBreakpoint"> Associated pending breakpoint. </param>
        public AD7BoundBreakpoint(AD7Engine engine, BP_REQUEST_INFO bpReqInfo, AD7PendingBreakpoint pendingBreakpoint)
        {
            if (bpReqInfo.bpLocation.bpLocationType == (uint)enum_BP_LOCATION_TYPE.BPLT_CODE_FILE_LINE)
            {
                string documentName;

                // Get Decument Position and File Name
                IDebugDocumentPosition2 docPosition = (IDebugDocumentPosition2)(Marshal.GetObjectForIUnknown(bpReqInfo.bpLocation.unionmember2));
                docPosition.GetFileName(out documentName);

                // Need to shorten the path we send to GDB.
                StringBuilder shortPath = new StringBuilder(1024);
                GetShortPathName(documentName, shortPath, shortPath.Capacity);

                // Get the location in the document that the breakpoint is in.
                TEXT_POSITION[] startPosition = new TEXT_POSITION[1];
                TEXT_POSITION[] endPosition = new TEXT_POSITION[1];
                docPosition.GetRange(startPosition, endPosition);

                _engine = engine;
                m_bpLocationType = (uint)enum_BP_LOCATION_TYPE.BPLT_CODE_FILE_LINE;
                m_filename = shortPath.ToString();
                m_line = startPosition[0].dwLine + 1;

                _pendingBreakpoint = pendingBreakpoint;
                _enabled = true;
                _deleted = false;
                _hitCount = 0;
                m_remoteID = _engine.BPMgr.RemoteAdd(this);
            }
            else if (bpReqInfo.bpLocation.bpLocationType == (uint)enum_BP_LOCATION_TYPE.BPLT_CODE_FUNC_OFFSET)
            {
                string func;

                IDebugFunctionPosition2 funcPosition = (IDebugFunctionPosition2)(Marshal.GetObjectForIUnknown(bpReqInfo.bpLocation.unionmember2));
                funcPosition.GetFunctionName(out func);

                _engine = engine;
                m_func = func;
                _enabled = true;
                _deleted = false;
                _hitCount = 0;
                m_bpLocationType = (uint)enum_BP_LOCATION_TYPE.BPLT_CODE_FUNC_OFFSET;
                _pendingBreakpoint = pendingBreakpoint;
                m_remoteID = _engine.BPMgr.RemoteAdd(this);
            }

//            if ((m_remoteID == 0) && (VSNDK.AddIn.VSNDKAddIn.isDebugEngineRunning == false))
            if (m_remoteID == 0)
            {
                return;
            }

            // Set the hit count and condition
            if (bpReqInfo.bpPassCount.stylePassCount != enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_NONE)
                SetPassCount(bpReqInfo.bpPassCount);
            if (bpReqInfo.bpCondition.styleCondition != enum_BP_COND_STYLE.BP_COND_NONE)
                SetCondition(bpReqInfo.bpCondition);

            // Get the Line Position sent back from GDB
            TEXT_POSITION tpos = new TEXT_POSITION();
            tpos.dwLine = _GDB_linePos - 1;

            uint xAddress = UInt32.Parse(_GDB_Address.Substring(2), System.Globalization.NumberStyles.HexNumber);

            AD7MemoryAddress codeContext = new AD7MemoryAddress(_engine, xAddress);
            AD7DocumentContext documentContext = new AD7DocumentContext(_GDB_filename, tpos, tpos, codeContext);

            _breakpointResolution = new AD7BreakpointResolution(_engine, xAddress, documentContext);

            _engine.Callback.OnBreakpointBound(this, 0);
        }


        /// <summary>
        ///  Sets the count and conditions upon which a breakpoint is fired. 
        ///  (http://msdn.microsoft.com/en-us/library/bb161364.aspx)
        /// </summary>
        /// <param name="bpPassCount"> Describes the count and conditions upon which a conditional breakpoint is fired. </param>
        /// <returns> VSConstants.S_OK if successful, VSConstants.S_FALSE if not. </returns>
        public int SetPassCount(BP_PASSCOUNT bpPassCount)
        {
            bool isRunning = false;
            int result = VSConstants.S_FALSE;
            while (!_engine.eDispatcher.lockedBreakpoint(this, true, false))
            {
                Thread.Sleep(0);
            }
            while (!_engine.eDispatcher.enterCriticalRegion())
            {
                Thread.Sleep(0);
            }
            if ((_engine.m_state == AD7Engine.DE_STATE.RUN_MODE) && (EventDispatcher._GDBRunMode == true))
            {
                isRunning = true;
                _engine.eDispatcher.PrepareToModifyBreakpoint();
            }
            m_bpPassCount = bpPassCount;
            if (bpPassCount.stylePassCount == enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_EQUAL_OR_GREATER)
            {
                _isHitCountEqual = false;
                _hitCountMultiple = 0;
                if (!_breakWhenCondChanged)
                {
                    if ((int)((bpPassCount.dwPassCount - _hitCount)) >= 0)
                    {
                        if (_engine.eDispatcher.ignoreHitCount(GDB_ID, (int)(bpPassCount.dwPassCount - _hitCount)))
                            result = VSConstants.S_OK;
                    }
                    else
                    {
                        if (_engine.eDispatcher.ignoreHitCount(GDB_ID, 1))
                            result = VSConstants.S_OK;
                    }

                }
                else
                    result = VSConstants.S_OK;
            }
            else if (bpPassCount.stylePassCount == enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_EQUAL)
            {
                _hitCountMultiple = 0;
                _isHitCountEqual = true;
                if (!_breakWhenCondChanged)
                {
                    if (_engine.eDispatcher.ignoreHitCount(GDB_ID, (int)(bpPassCount.dwPassCount - _hitCount)))
                        result = VSConstants.S_OK;
                }
                else
                    result = VSConstants.S_OK;
            }
            else if (bpPassCount.stylePassCount == enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_MOD)
            {
                _isHitCountEqual = false;
                _hitCountMultiple = bpPassCount.dwPassCount;
                if (!_breakWhenCondChanged)
                {
                    if (_engine.eDispatcher.ignoreHitCount(GDB_ID, (int)(_hitCountMultiple - (_hitCount % _hitCountMultiple))))
                        result = VSConstants.S_OK;
                }
                else
                    result = VSConstants.S_OK;
            }
            else if (bpPassCount.stylePassCount == enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_NONE)
            {
                _isHitCountEqual = false;
                _hitCountMultiple = 0;
                if (!_breakWhenCondChanged)
                {
                    if (_engine.eDispatcher.ignoreHitCount(GDB_ID, 1)) // ignoreHitCount decrement by 1 automatically, so sending 1 means to stop ignoring (or ignore 0)
                        result = VSConstants.S_OK;
                }
                else
                    result = VSConstants.S_OK;
            }

            if (isRunning)
            {
                isRunning = false;
                _engine.eDispatcher.ResumeFromInterrupt();
            }

            _engine.eDispatcher.leaveCriticalRegion();
            _engine.eDispatcher.unlockBreakpoint(this, true, false);
            return result;
        }


        /// <summary>
        /// Sets the conditions under which a conditional breakpoint fires. (http://msdn.microsoft.com/en-us/library/bb146215.aspx)
        /// </summary>
        /// <param name="bpCondition"> Describes the conditions under which a breakpoint fires. </param>
        /// <returns> VSConstants.S_OK if successful, VSConstants.S_FALSE if not. </returns>
        public int SetCondition(BP_CONDITION bpCondition)
        {
            bool updatingCondBreak = this._engine.m_updatingConditionalBreakpoint.WaitOne(0);
            bool isRunning = false;
            bool verifyCondition = false;
            int result = VSConstants.S_FALSE;
            while (!_engine.eDispatcher.lockedBreakpoint(this, false, true))
            {
                Thread.Sleep(0);
            }

            if (_hitCount != 0)
            {
                _engine.eDispatcher.resetHitCount(this, false);
            }

            while (!_engine.eDispatcher.enterCriticalRegion())
            {
                Thread.Sleep(0);
            }

            if ((_engine.m_state == AD7Engine.DE_STATE.RUN_MODE) && (EventDispatcher._GDBRunMode == true))
            {
                isRunning = true;
                _engine.eDispatcher.PrepareToModifyBreakpoint();
                _engine.m_state = AD7Engine.DE_STATE.BREAK_MODE;
            }

            m_bpCondition = bpCondition;

            if (bpCondition.styleCondition == enum_BP_COND_STYLE.BP_COND_WHEN_TRUE)
            {
                if (_breakWhenCondChanged)
                {
                    _breakWhenCondChanged = false;
                    verifyCondition = true;
                }
                else
                    _breakWhenCondChanged = false;

                _previousCondEvaluation = "";
                if (_engine.eDispatcher.setBreakpointCondition(GDB_ID, bpCondition.bstrCondition))
                    result = VSConstants.S_OK;
            }
            else if (bpCondition.styleCondition == enum_BP_COND_STYLE.BP_COND_WHEN_CHANGED)
            {
                _breakWhenCondChanged = true;
                _previousCondEvaluation = bpCondition.bstrCondition; // just to initialize this variable
                _engine.eDispatcher.ignoreHitCount(GDB_ID, 1); // have to break always to evaluate this option because GDB doesn't support it.
                _engine.eDispatcher.setBreakpointCondition(GDB_ID, "");

                result = VSConstants.S_OK;
            }
            else if (bpCondition.styleCondition == enum_BP_COND_STYLE.BP_COND_NONE)
            {
                if (_breakWhenCondChanged)
                {
                    _breakWhenCondChanged = false;
                    verifyCondition = true;
                }
                else
                    _breakWhenCondChanged = false;

                _previousCondEvaluation = "";
                if (_engine.eDispatcher.setBreakpointCondition(GDB_ID, ""))
                    result = VSConstants.S_OK;
            }

            _engine.eDispatcher.leaveCriticalRegion();
            _engine.eDispatcher.unlockBreakpoint(this, false, true);

            if (verifyCondition)
            {
                SetPassCount(m_bpPassCount);
                verifyCondition = false;
            }

            if (isRunning)
            {
                isRunning = false;
                _engine.m_state = AD7Engine.DE_STATE.RUN_MODE;
                _engine.eDispatcher.ResumeFromInterrupt();
            }

            this._engine.m_updatingConditionalBreakpoint.Set();

            return result;
        }

        #region IDebugBoundBreakpoint2 Members


        /// <summary>
        /// Called when the breakpoint is being deleted by the user. (http://msdn.microsoft.com/en-us/library/bb146595.aspx)
        /// </summary>
        /// <returns> VSConstants.S_OK </returns>
        int IDebugBoundBreakpoint2.Delete()
        {
            if (!_deleted)
            {
                _enabled = false;
                _deleted = true;
                _pendingBreakpoint.OnBoundBreakpointDeleted(this);                
                _engine.BPMgr.RemoteDelete(this);
                m_remoteID = -1;
            }
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Called by the debugger UI when the user is enabling or disabling a breakpoint. 
        /// (http://msdn.microsoft.com/en-us/library/bb145150.aspx)
        /// </summary>
        /// <param name="fEnable"> Equal to 0 if disabling; different than 0 if enabling. </param>
        /// <returns> VSConstants.S_OK </returns>
        int IDebugBoundBreakpoint2.Enable(int fEnable)
        {
            bool xEnabled = fEnable != 0;
            if (_enabled != xEnabled)
            {
                if (xEnabled)
                {
                    _engine.BPMgr.RemoteEnable(this);
                }
                else
                {
                    _engine.BPMgr.RemoteDisable(this);
                }
                _enabled = xEnabled;
            }
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Return the breakpoint resolution which describes how the breakpoint bound in the debuggee. 
        /// (http://msdn.microsoft.com/en-us/library/bb145891.aspx)
        /// </summary>
        /// <param name="ppBPResolution"> Contains the information that describes a bound breakpoint. </param>
        /// <returns> VSConstants.S_OK </returns>
        int IDebugBoundBreakpoint2.GetBreakpointResolution(out IDebugBreakpointResolution2 ppBPResolution)
        {
            ppBPResolution = _breakpointResolution;
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Return the pending breakpoint for this bound breakpoint. (http://msdn.microsoft.com/en-us/library/bb145337.aspx)
        /// </summary>
        /// <param name="ppPendingBreakpoint"> Contains a breakpoint that is ready to bind to a code location.</param>
        /// <returns> VSConstants.S_OK </returns>
        int IDebugBoundBreakpoint2.GetPendingBreakpoint(out IDebugPendingBreakpoint2 ppPendingBreakpoint)
        {
            ppPendingBreakpoint = _pendingBreakpoint;
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Gets the state of this bound breakpoint. (http://msdn.microsoft.com/en-us/library/bb161276.aspx)
        /// </summary>
        /// <param name="pState"> Describes the state of the breakpoint. </param>
        /// <returns> VSConstants.S_OK </returns>
        int IDebugBoundBreakpoint2.GetState(enum_BP_STATE[] pState)
        {
            pState[0] = 0;

            if (_deleted)
            {
                pState[0] = enum_BP_STATE.BPS_DELETED;
            }
            else if (_enabled)
            {
                pState[0] = enum_BP_STATE.BPS_ENABLED;
            }
            else if (!_enabled)
            {
                pState[0] = enum_BP_STATE.BPS_DISABLED;
            }

            return VSConstants.S_OK;
        }


        /// <summary>
        /// Gets the current hit count for this bound breakpoint. (http://msdn.microsoft.com/en-us/library/bb145340.aspx)
        /// </summary>
        /// <param name="pdwHitCount">Returns the hit count. </param>
        /// <returns> AD7_HRESULT.E_BP_DELETED if the breakpoint was deleted; or VSConstants.S_OK if not. </returns>
        int IDebugBoundBreakpoint2.GetHitCount(out uint pdwHitCount)
        {
            if (_deleted)
            {
                pdwHitCount = 0;
                return AD7_HRESULT.E_BP_DELETED;
            }
            else
            {
                pdwHitCount = _hitCount;
                return VSConstants.S_OK;
            }
        }


        /// <summary>
        /// Sets or changes the condition associated with this bound breakpoint. (http://msdn.microsoft.com/en-us/library/bb146215.aspx)
        /// </summary>
        /// <param name="bpCondition"> Describes the condition. </param>
        /// <returns> VSConstants.S_OK if successful, VSConstants.S_FALSE if not. </returns>
        int IDebugBoundBreakpoint2.SetCondition(BP_CONDITION bpCondition)
        {
            return SetCondition(bpCondition);
        }


        /// <summary>
        /// Sets the hit count for the bound breakpoint. (http://msdn.microsoft.com/en-us/library/bb146736.aspx)
        /// </summary>
        /// <param name="dwHitCount"> The hit count to set. </param>
        /// <returns> AD7_HRESULT.E_BP_DELETED if the breakpoint was deleted; or VSConstants.S_OK if not. </returns>
        int IDebugBoundBreakpoint2.SetHitCount(uint dwHitCount)
        {
            if (_deleted)
            {                
                return AD7_HRESULT.E_BP_DELETED;
            }
            else
            {
                if ((dwHitCount == 0) && (_hitCount != 0))
                {
                    _hitCount = dwHitCount;
                    _engine.eDispatcher.resetHitCount(this, true);
                }
                else
                    _hitCount = dwHitCount;
                return VSConstants.S_OK;
            }
        }


        /// <summary>
        /// Sets or changes the pass count associated with this bound breakpoint. (http://msdn.microsoft.com/en-us/library/bb161364.aspx)
        /// </summary>
        /// <param name="bpPassCount"> Specifies the pass count. </param>
        /// <returns> VSConstants.S_OK if successful, VSConstants.S_FALSE if not. </returns>
        int IDebugBoundBreakpoint2.SetPassCount(BP_PASSCOUNT bpPassCount)
        {
            return SetPassCount(bpPassCount);
        }

        #endregion
    }
}
