using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zakuBlink2
{
    public class GetterPerformanceMonitor
    {

        /// <summary>
        /// LogocDiskのパフォーマンスモニター値を取得します
        /// </summary>
        /// <param name="counterName">カウンター名</param>
        /// <returns></returns>
        public static PerformanceCounter GetInstancePerformanceMonitorDiskAccess(String counterName)
        {
            var outputInstance = new PerformanceCounter();
            ((System.ComponentModel.ISupportInitialize)(outputInstance)).BeginInit();
            outputInstance.CategoryName = "LogicalDisk";
            outputInstance.CounterName = counterName;
            outputInstance.InstanceName = "_Total";
            ((System.ComponentModel.ISupportInitialize)(outputInstance)).EndInit();
            return outputInstance;
        }

    }
}
