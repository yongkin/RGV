



namespace SafeRun.WPFCtrlEquip.WPFRunner
{
    public partial class DashboardDetailFormModel : RunerViewModelBase
    {

        [ObservableProperty]
        public ObservableCollection<FrmData> frmDatas;

        public DashboardDetailFormModel()
        {
            FrmDatas = new ObservableCollection<FrmData>();

        }

        public void InitFrmData(BizContext biz)
        {
            foreach (var nodeItem in biz.Block.Nodes)
            {
                var frmData = new FrmData();
                frmData.NodeId = nodeItem.Id;
                frmData.NodeName = nodeItem.Name;
                //frmData.nodeValue = "";
                //frmData.valueTime = "";
                //frmData.isExpired = "";
                //frmData.WriteValue = "";
                //frmData.WriteTime = "";
                frmData.BlackParam = biz.Block.Param;
                frmData.NodeCode = nodeItem.Code;
                FrmDatas.Add(frmData);
            }

            XTask.Loop(() =>
            {
                try { RefreshFrmData(biz); } catch (Exception ex) {  }
            }, 500);

        }

        public void RefreshFrmData(BizContext biz)
        {

            //if (biz.Equip == null)
            //{
            //    //this.tbEquipStatus.Text = "设备创建失败 equip is null\r\n" + biz.MDEInfo.ToIndentedJson();
            //    return;
            //}
            
            var keys = new List<string>();
            foreach (var n in biz.Block.Nodes)
            {
                keys.Add(n.Id);
            }

            //更新节点值
            var values = EquipContextHelper.Instance.GetKeyValuesFromStore(keys.ToArray());
            foreach(var item in values)
            {
                var updItem = FrmDatas.FirstOrDefault(r => r.NodeId.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase));
                if (updItem == null) continue;
                updItem.NodeValue = item.Value;
                updItem.ValueTime = item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
                updItem.IsExpired = item.IsExpired.ToString();
            }

            //更新写入历史
            var history = EquipContextHelper.Instance.GetWriteHistory(keys.ToArray());
            foreach (var item in history)
            {
                var updItem = FrmDatas.FirstOrDefault(r => r.NodeId.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase));
                if (updItem == null) continue;
                updItem.WriteValue = item.Value;
                updItem.WriteTime = item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }

        }

    }


    public partial class FrmData : RunerViewModelBase
    {
        public BizContext biz { get; set; }

        [ObservableProperty]
        public string nodeId;

        [ObservableProperty]
        public string nodeName;

        [ObservableProperty]
        public string nodeValue;

        [ObservableProperty]
        public string valueTime;

        [ObservableProperty]
        public string isExpired;

        [ObservableProperty]
        public string writeValue;

        [ObservableProperty]
        public string writeTime;

        [ObservableProperty]
        public string blackParam;

        [ObservableProperty]
        public string nodeCode;

    }
}
