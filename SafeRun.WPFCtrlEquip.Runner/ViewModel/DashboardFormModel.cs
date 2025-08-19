


using MicroSvr.Comm;
using MicroSvr.RPC.Client;
using SafeRun.CtrlBase;
using SafeRun.CtrlComm;
using SafeRun.WMS.Spi;
using SafeRun.WPFCtrlEquip.WPFRunner.Common;
using SafeRun.WPFCtrlEquip.WPFRunner.ControlViews;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace SafeRun.WPFCtrlEquip.WPFRunner
{
    public partial class DashboardFormModel : RunerViewModelBase
    {
        public DashboardFormModel()
        {
            BlockInfoList = new ObservableCollection<_BlockInfo>();
            CreateListItem();
            InitTaskStart();
            InitEquipmentError();

            XTask.Loop(() =>
            {
                try
                {

                    if (autoRefresh)
                    {
                        refreshListItemData();
                    }
                }
                catch (Exception ex) { }
            }, 500);
        }

        #region 刷新数据监控页面
        [ObservableProperty]
        public ObservableCollection<_BlockInfo> blockInfoList;
        public Dictionary<string, BizNode> BlockKeys { get; set; }

        public void dataGridEffect_SelectionChanged(object blockInfoObj)
        {
            var _blockInfo = blockInfoObj as _BlockInfo;
        }

        [ObservableProperty]
        private bool autoRefresh;

        

        private void CreateListItem()
        {
            var cs = RouteRpcApi.EquipDataApi().GetMdcBizs(XApp.Current.AppId);

            SafeRun.CtrlBiz.SpiService.BizContext context = null;
            foreach (var c in cs)
            {
                //BizContextStore.Instance.AddContext(new BizContext() { MdcBiz = c });
                context = new SafeRun.CtrlBiz.SpiService.BizContext() { MdcBiz = c };
            }

            BlockKeys = new Dictionary<string, BizNode>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var nodeItem in context.MdcBiz.Nodes)
            {
                //var nodeItem = context.MdcBiz.Nodes.FirstOrDefault(m => m.NodeName == nodeName);
                if (nodeItem == null) continue;
                BlockKeys.TryAdd(nodeItem.BlockNodeId, nodeItem);

            }

            refreshListItemData();

            //if (context == null) return null;

            //2.拼接需要的节点
            //var keyVals = new Dictionary<string, BizNode>(StringComparer.CurrentCultureIgnoreCase);
            //foreach (var nodeName in demandNodes)
            //{
            //    var nodeItem = context.MdcBiz.Nodes.FirstOrDefault(m => m.NodeName == nodeName);
            //    if (nodeItem == null) continue;
            //    keyVals.TryAdd(nodeItem.BlockNodeId, nodeItem);
            //}
            #region PLC数据获取，调整为查询业务 2025-02-26 hyx
            /* PLC数据获取，调整为查询业务 2025-02-26 hyx
            var bizs = EquipContextHelper.Instance.GetContexts();
            //var bizs = RouteRpcApi.CtrlEquipContextApi().GetContexts();
            if (bizs == null || bizs.Length == 0)
            {
                return;
            }
            bizs = bizs.OrderBy(r => r.Block.Index).ThenBy(r => r.MDEInfo.Name).ToArray();
            int i = 0;

            foreach (var biz in bizs)
            {
                i++;
                var blockNode = new _BlockInfo();
                blockNode.LineId = i;
                blockNode.biz = biz;
                blockNode.EquipId = biz.MDEInfo.Id;
                blockNode.EquipName = biz.MDEInfo.Name;
                blockNode.EquipImpl = biz.MDEInfo.Impl;
                blockNode.BlockId = biz.Block.Id;
                blockNode.BlockLen = biz.Block.Len;
                blockNode.ReadHz = biz.Block.ReadHz;
                var param = JsonExtend.ToJObject(biz.Block.Param);
                blockNode.BlockStart = (param["BlockId"] ?? "").ToString();

                BlockInfoList.Add(blockNode);
            }*/
            #endregion

        }

        /// <summary>
        /// 刷新按钮点击
        /// </summary>
        [RelayCommand]
        public void refreshListItem()
        {
            refreshListItemData();
        }


        /// <summary>
        /// 数据刷新
        /// </summary>
        public void refreshListItemData()
        {
            try
            {
                if (BlockKeys.Count <= 0)
                {
                    CreateListItem();
                }

                var kvs = RouteRpcApi.GetRemoteNodes(BlockKeys.Keys.ToArray());

                foreach (var key in BlockKeys.Keys)
                {
                    var item = kvs.FirstOrDefault(m => m.Key == key);
                    if (item == null) continue;
                    var nodeItem = BlockKeys[key];
                    var updItem = blockInfoList.FirstOrDefault(m => m.bizNodeId == nodeItem.BizNodeId);

                    if (updItem == null)
                    {
                        blockInfoList.Add(new _BlockInfo()
                        {
                            BizNodeId = nodeItem.BizNodeId,
                            NodeName = nodeItem.NodeName,
                            //NodeCName = nodeItem.NodeCName,
                            Value = item.Value,
                            Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff")
                        });
                    }
                    else
                    {
                        var _item = updItem as _BlockInfo;
                        _item.BizNodeId = nodeItem.BizNodeId;
                        _item.NodeName = nodeItem.NodeName;
                        _item.Value = item.Value;
                        _item.Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }

                    //result.Add((key: nodeItem.NodeName, val: item.Value));
                }


                /* hyx 2025-02-26 改成查询业务节点显示
                var newBizDatas = EquipContextHelper.Instance.GetContexts();
                //var newBizDatas = RouteRpcApi.CtrlEquipContextApi().GetContexts();

                foreach (var item in BlockInfoList)
                {
                    var _item = item as _BlockInfo;
                    if (_item == null || _item.biz == null)
                    {
                        continue;
                    }

                    //取出最新的数据
                    var updateBiz = newBizDatas.FirstOrDefault(r => r.Block.Id == _item.blockId);

                    if (updateBiz == null || updateBiz.RunData == null)
                    {
                        continue;
                    }
                    _item.biz = updateBiz;

                    //更新页面数据
                    _item.BeginTime = _item.biz.RunData.BeginTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    _item.ExcuteMilliseconds = _item.biz.RunData.ExcuteMilliseconds.ToString();
                    _item.BeginCount = _item.biz.RunData.BeginCount;
                    var equipSataus = string.Empty;
                    if (_item.biz.Equip != null)
                    {
                        equipSataus = _item.biz.Equip.Status.Code.ToString();
                    }
                    _item.Status = equipSataus;
                    _item.SuccessCount = _item.biz.RunData.SuccessCount;
                    _item.FailedCount = _item.biz.RunData.FailedCount;
                    if(_item.FailedCount > 0)
                    {
                        _item.FailedSustainedTime = _item.biz.RunData.FailedSustainedTime.ToString("HH:mm:ss.fff");
                    }
                    
                    _item.BeginTime = _item.biz.RunData.BeginTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                */
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 刷新按钮点击
        /// </summary>
        [RelayCommand]
        public void ClickFullLayout()
        {
            // 创建辅助窗口
            // 加载布局页面: selItem.biz.Block.Name = MDC_EQUIP_BLOCK.BLOCK_NAME
            var auxiliaryWindow = new Window
            {
                //Title = "Auxiliary Page",
                //Content = new ControlViews.Page1() // 设置窗口内容为辅助页面
                Title = "",
                //Content = new FT_FullLayout_CC(),
                WindowState = WindowState.Maximized,
                //WindowStyle = WindowStyle.SingleBorderWindow,
                //ResizeMode = ResizeMode.NoResize,

            };
            auxiliaryWindow.Content = new FT_FullLayout_CC(auxiliaryWindow);
            //auxiliaryWindow.Content = new TestPage1();
            // 显示辅助窗口
            auxiliaryWindow.ShowDialog();
        }

        #endregion


        #region 执行任务页面相关

        [ObservableProperty]
        public ObservableCollection<_TaskInfo> taskInfoList = new ObservableCollection<_TaskInfo>();

        [ObservableProperty]
        public ObservableCollection<KeyValuePair<string, string>> taskStatusComB;
        private void InitTaskStart()
        {
            TaskStatusComB = new ObservableCollection<KeyValuePair<string, string>>();
            TaskStatusComB.Add(new KeyValuePair<string, string>("0", "未执行 "));
            TaskStatusComB.Add(new KeyValuePair<string, string>("1", "执行中 "));
            TaskStatusComB.Add(new KeyValuePair<string, string>("2", "执行完成 "));
            TaskStatusComB.Add(new KeyValuePair<string, string>("3", "执行异常 "));
            TaskStatusComB.Add(new KeyValuePair<string, string>("", "全部 "));
        }

        /// <summary>
        /// 查询执行任务信息
        /// </summary>
        /// <param name="dicQueryCriteria"></param>
        public void TaskQuery(Dictionary<string, string> dicQueryCriteria)
        {
            TaskInfoList.Clear();

            var wcsTask = RouteRpcApi.WMSTaskApi();
            var queryCriteria = new WCSTask();
            foreach (var item in dicQueryCriteria)
            {
                if (string.IsNullOrEmpty(item.Value)) continue;
                switch (item.Key)
                {
                    case "TaskNo":
                        queryCriteria.TaskNo = int.Parse(item.Value);
                        break;
                    case "TaskStatus":
                        queryCriteria.ExecStatus = int.Parse(item.Value);
                        break;
                    case "SLocNo":
                        queryCriteria.SLocNo = (item.Value);
                        break;
                    case "DLocNo":
                        queryCriteria.DLocNo = (item.Value);
                        break;
                }
            }

            var taskData = wcsTask.GetTaskList(queryCriteria);

            foreach (var item in taskData)
            {
                var taskItem = new _TaskInfo();
                taskItem.Id = item.Id;
                taskItem.TaskNo = item.TaskNo.ToString();
                //taskItem.TaskType = item.TaskType;
                taskItem.TaskType = item.TaskTypeName;
                taskItem.ExecBTime = item.CreateTime ?? "";
                taskItem.TaskQty = $"{item.TaskQty ?? 0}";
                taskItem.SLocNo = item.SLocNo ?? "";
                taskItem.DLocNo = item.DLocNo ?? "";
                taskItem.CurrLocNo = item.CurrLocNo ?? "";
                taskItem.MaterCode = item.MaterCode ?? "";
                taskItem.ErrCode = item.ErrCode ?? "";
                taskItem.ErrDesc = item.ErrDesc ?? "";

                TaskInfoList.Add(taskItem);
            }

        }


        #endregion

        #region 异常记录
        [ObservableProperty]
        public ObservableCollection<_EquipmentError> equipmentErrorItems;

        private ObservableCollection<_EquipmentError> _pagedItems;

        [ObservableProperty]
        private DateTime errorStartDate;

        [ObservableProperty]
        private DateTime errorEndDate;

        #region 设备类型
        [ObservableProperty]
        private KeyValuePair<string, string> selectedPlcCode;

        [ObservableProperty]
        public ObservableCollection<KeyValuePair<string, string>> plcCodeComB;
        private void InitplcCode()
        {
            PlcCodeComB = new ObservableCollection<KeyValuePair<string, string>>();
            PlcCodeComB.Add(new KeyValuePair<string, string>("CRN", "堆垛机 "));
            PlcCodeComB.Add(new KeyValuePair<string, string>("CV", "线体"));
        }
        #endregion


        private int _currentPage;
        private int _itemsPerPage;
        private int _totalPages;

        public List<_EquipmentError> GetEquipmentErrorRecords(int pageIndex, int pageSize,string plcCode,DateTime? startTime, DateTime? endTime)
        {
            var queryParam = new CommonQuery();
            queryParam.pageinfo = new PageConfig() { pageindex = pageIndex, pagesize = pageSize };

            if (!string.IsNullOrEmpty(plcCode))
            {
                queryParam.PlcCode = plcCode;
            }
            if (startTime != null)
            {
                queryParam.StartTime = startTime;
            }
            if (endTime != null)
            {
                queryParam.EndTime = endTime;
            }

            var datas = RouteRpcApi.WMSErrorAlarmApi().GetWCSErrorAlarmList(queryParam);
            if (datas == null || datas.rows == null )
            {
                return null;
            }

            _totalPages = datas.total;
            _currentPage = datas.start;
            _itemsPerPage = datas.len;

            //if(!(datas.rows is List<WCSErrorAlarm>)) return null;
            //dynamic errorRecords = datas.rows;

            var errorRecords = JsonSerializer.Deserialize<List<ErrorDetail>>(datas.rows.ToJson());


            var errorList = new List<_EquipmentError>();
            foreach (var item in errorRecords)
            {
                var errorItem = new _EquipmentError();
                errorItem.id = item.id;
                errorItem.plcCode = item.plcCode;
                errorItem.plcName = item.plcName;
                errorItem.errCode = item.errCode;
                errorItem.errDetails = item.errDetails;
                errorItem.errorDate = item.errorDate;
                errorItem.errName = item.errName;
                errorItem.orderNo = item.orderNo;
                errorItem.errExplain = item.errExplain;
                errorItem.taskNo = item.taskNo;

                errorList.Add(errorItem);
            }
            return errorList;
        }

        public void InitEquipmentError()
        {
            InitplcCode();
            InitEquipmentErrorView();
        }


        public void InitEquipmentErrorView()
        {
            ErrorStartDate = DateTime.Now;
            ErrorEndDate = DateTime.Now;

            DateTime.TryParse($"{ErrorStartDate.ToString("yyyy-MM-dd")} 00:00:00", out DateTime startTime);
            DateTime.TryParse($"{ErrorEndDate.ToString("yyyy-MM-dd")} 23:59:59", out DateTime endTime);

            EquipmentErrorItems = new ObservableCollection<_EquipmentError>();
            _pagedItems = new ObservableCollection<_EquipmentError>();
            _currentPage = 1;
            _itemsPerPage = 10;

            var errorRecords = GetEquipmentErrorRecords(_currentPage, _itemsPerPage, "", startTime, endTime);

            if (errorRecords == null) return;
            // 插入是数据
            foreach (var item in errorRecords)
            {
                equipmentErrorItems.Add(item);
            }

            //UpdatePagedItems();
        }

        public void UpdatePagedItems()
        {
            var plcCode = SelectedPlcCode.Key;

            DateTime.TryParse($"{ErrorStartDate.ToString("yyyy-MM-dd")} 00:00:00", out DateTime startTime);
            DateTime.TryParse($"{ErrorEndDate.ToString("yyyy-MM-dd")} 23:59:59", out DateTime endTime);

            var errorRecords = GetEquipmentErrorRecords(_currentPage, _itemsPerPage, plcCode, startTime, endTime);
            // 插入是数据
            equipmentErrorItems.Clear();
            foreach (var item in errorRecords)
            {
                equipmentErrorItems.Add(item);
            }
        }

        public ObservableCollection<_EquipmentError> PagedItems
        {
            get { return _pagedItems; }
            set
            {
                _pagedItems = value;
                OnPropertyChanged(nameof(PagedItems));
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePagedItems();
            }
        }

        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set
            {
                _itemsPerPage = value;
                OnPropertyChanged(nameof(ItemsPerPage));
                UpdatePagedItems();
            }
        }

        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public ICommand NextPageCommand => new RelayCommand(NextPage, CanGoToNextPage);
        public ICommand PreviousPageCommand => new RelayCommand(PreviousPage, CanGoToPreviousPage);

        

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }
        }

        private bool CanGoToNextPage()
        {
            return CurrentPage < TotalPages;
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private bool CanGoToPreviousPage()
        {
            return CurrentPage > 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public partial class _EquipmentError : RunerViewModelBase
    {
        [ObservableProperty]
        public string id;

        [ObservableProperty]
        public string plcCode;

        [ObservableProperty]
        public string plcName;

        [ObservableProperty]
        public string errCode;

        [ObservableProperty]
        public string errDetails;

        [ObservableProperty]
        public string errorDate;

        [ObservableProperty]
        public string errName;

        [ObservableProperty]
        public string orderNo;

        [ObservableProperty]
        public string errExplain;

        [ObservableProperty]
        public string taskNo;
    }

    public class ErrorDetail
    {
        public string id { get; set; }
        public string plcCode { get; set; }
        public string plcName { get; set; }
        public string errCode { get; set; }
        public string errDetails { get; set; }
        public string errorDate { get; set; }
        public string errName { get; set; }
        public string orderNo { get; set; }
        public string errExplain { get; set; }
        public string taskNo { get; set; }
    }

    public partial class _BlockInfo : RunerViewModelBase
    {
        [ObservableProperty]
        public string bizNodeId;

        [ObservableProperty]
        public string nodeName;

        [ObservableProperty]
        public string nodeCName;

        [ObservableProperty]
        public string value;

        [ObservableProperty]
        public string time;


        /* 2025-02-26 hyx 不需要显示这些数据
        public BizContext biz { get; set; }

        [ObservableProperty]
        public int lineId;

        [ObservableProperty]
        public string equipId;

        [ObservableProperty]
        public string equipName;

        [ObservableProperty]
        public string equipImpl;

        [ObservableProperty]
        public string blockId;

        [ObservableProperty]
        public string blockStart;

        [ObservableProperty]
        public int blockLen;

        [ObservableProperty]
        public int readHz;

        [ObservableProperty]
        public string status;

        [ObservableProperty]
        public string beginTime;

        [ObservableProperty]
        public string excuteMilliseconds;

        [ObservableProperty]
        public int beginCount;

        [ObservableProperty]
        public int successCount;

        [ObservableProperty]
        public int failedCount;

        [ObservableProperty]
        public string failedSustainedTime;

        #region 其他绑定方案
        //public class _BlockInfo : INotifyPropertyChanged

        //private int _leginCount;
        //public int BeginCount { get { return _leginCount; } 
        //    set
        //    {
        //        if (_leginCount != value)
        //        {
        //            _leginCount = value;
        //            OnPropertyChanged("BeginCount");
        //        }
        //    }
        //}
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        #endregion
        */
    }

    public partial class _TaskInfo : RunerViewModelBase
    {
        [ObservableProperty]
        public long? id;

        [ObservableProperty]
        public string taskNo;

        [ObservableProperty]
        public string taskType;

        [ObservableProperty]
        public string execBTime;

        [ObservableProperty]
        public string taskQty;

        [ObservableProperty]
        public string sLocNo;

        [ObservableProperty]
        public string dLocNo;

        [ObservableProperty]
        public string currLocNo;

        [ObservableProperty]
        public string materCode;

        [ObservableProperty]
        public string errCode;

        [ObservableProperty]
        public string errDesc;
    }
}
