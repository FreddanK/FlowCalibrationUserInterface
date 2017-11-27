using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DataExporter
    {
        public static void SaveTimeAndValuesToCsv(IList<Double> times, IList<Double> values, String filePath)
        {
            if (times.Count() != values.Count())
            {
                throw new Exception("Input lists not of equal length");
            }

            StringBuilder stringBuilder = new StringBuilder();

            for(int i=0; i< values.Count(); i++)
            {
                String time = times[i].ToString(CultureInfo.InvariantCulture);
                String value = values[i].ToString(CultureInfo.InvariantCulture);
                String line = string.Format("{0},{1}", time, value);
                stringBuilder.AppendLine(line);
            }

            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        public static void LoadTimeAndValuesFromCsv(IList<Double> times, IList<Double> values, String filePath)
        {
            times.Clear();
            values.Clear();
            String [] lines = File.ReadAllLines(filePath);
            foreach(String line in lines)
            {
                String[] entries = line.Split(',');
                Double time = 0;
                Double value = 0;
                Double.TryParse(entries[0], NumberStyles.Number, CultureInfo.InvariantCulture, out time);
                Double.TryParse(entries[1], NumberStyles.Number, CultureInfo.InvariantCulture, out value);

                times.Add(time);
                values.Add(value);
            }
        }
    }
}
