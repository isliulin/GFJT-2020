﻿using System;
using System.IO;
using QRST_DI_DS_Metadata.MetaDataCls;
using QRST_DI_DS_Metadata.Paths;
using log4net;
using System.Windows.Forms;
using QRST_DI_SS_DBInterfaces.IDBEngine;
 
namespace QRST_DI_MS_Component_DataImportorUI.Raster
{
    public class SingleDataImageProd
    {
        public QRST_DI_DS_Metadata.MetaDataCls.MetaDataImageProd _MetaData = null;
        public DirectoryInfo _PorDir = null;
        public bool _hasReadMetaData;
        ILog log = LogManager.GetLogger(typeof(SingleDataImageProd));
        public bool _hasImported = false;
        public static bool _CoverImported=false;     //已入库数据再次入库时默认同意执行二次
        public SingleDataImageProd(DirectoryInfo di)
        {
            _PorDir = di;
            _MetaData = new QRST_DI_DS_Metadata.MetaDataCls.MetaDataImageProd();
            _hasReadMetaData = false;
        }

        
        public void ReadMetaData()
        {
            _MetaData.ReadAttributes(_PorDir.FullName);
            _hasReadMetaData = true;
        }

        /// <summary>
        /// 执行单文件数据入库
        /// </summary>
        public void DataImport(IDbBaseUtilities indbUtil)
        {
            if (_hasImported)
            {
                System.Windows.Forms.DialogResult dr = MessageBox.Show(string.Format("数据 [{0}] 已经成功入库过是否继续入库？", this.ToString()), "提示", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No)
                {
                    return;
                }
            }
            if (!_hasReadMetaData)
            {
                ReadMetaData();
            }
            if (_MetaData.GroupCode == null || _MetaData.GroupCode == "")
            {
                _MetaData.GroupCode = MetaDataImageProd.GetDefaultGroupCode(indbUtil);
            }
            _MetaData.UploadDate = DateTime.Now;
            
            _MetaData.ImportData(indbUtil);
            _MetaData.GetModel(_MetaData.QRST_CODE, indbUtil);
            string destPath = GetDestDirPath();
            ImportProdDirFiles(destPath);

            _hasImported = true;
        }


        private string GetDestDirPath()
        {
            if (_MetaData.IsCreated)
            {
                string tableCode = StoragePath.GetTableCodeByQrstCode(_MetaData.QRST_CODE);
                StoragePath storePath = new StoragePath(tableCode);
                return storePath.GetNewDataPath(_MetaData);     //耗时很久
            }
            else
            {
                return "";
            }
        }

        public override string ToString()
        {
            return _PorDir.FullName;
        }
        private void ImportProdDirFiles(string destDir)
        {
            CopyDir(_PorDir, destDir);
        }

        private void CopyDir(DirectoryInfo srcdi, string destdir)
        {
            if (Directory.Exists(destdir))
            {
                Directory.Delete(destdir, true);
            }
            Directory.CreateDirectory(destdir);

            FileInfo[] fis = srcdi.GetFiles();

            for (int i = 0; i < fis.Length; i++)
            {
                try
                {
                    log.Info(string.Format("开始导入文件：{0}", fis[i].FullName));
                    fis[i].CopyTo(string.Format(@"{0}\{1}", destdir, fis[i].Name));
                    log.Info(string.Format("完成导入文件：{0}", fis[i].FullName));
                }
                catch (Exception ex)
                {
                    log.Info(string.Format("导入文件异常：{0}\r\n{1}", fis[i].FullName, ex.Message));
                }
            }

            DirectoryInfo[] dis = srcdi.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                try
                {
                    CopyDir(di, string.Format(@"{0}\{1}", destdir, di.Name));
                }
                catch{ }
            }
        }
        
        public static bool IsProdDir(DirectoryInfo di)
        {
            if (di.Exists)
            {
                if (di.Name.Contains("#"))
                {
                    return true;
                }
            }
            return false;
        }

    }
}