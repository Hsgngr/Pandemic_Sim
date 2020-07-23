using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SIR : MonoBehaviour
{
    private Text text;
    private PandemicArea pandemicArea;

   public StringBuilder sb = new System.Text.StringBuilder();
 

    // Start is called before the first frame update
    void Start()
    {
        pandemicArea = GetComponentInParent<PandemicArea>();
        text = GetComponent<Text>();
        sb.AppendLine("HealthyCount;InfectedCount;RecoveredCount;Time");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        text.text = "Total Healthy Agents = " + pandemicArea.healthyCounter + "\n" +
                    "Total Infected Agents =" + pandemicArea.infectedCounter;
        
    }

    public string ToCSV()
    {
        decimal time = Decimal.Round((decimal)Time.time, 2);
        sb.AppendLine(pandemicArea.healthyCounter.ToString() + ';' + pandemicArea.infectedCounter.ToString() + ";" + pandemicArea.recoveredCounter.ToString() +  ";" + time.ToString());
        return sb.ToString();

    }
    public void SaveToFile()
    {
        // Use the CSV generation from before
        var content = ToCSV();

        // The target file path e.g.
        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);


        var filePath = Path.Combine(folder, "export.csv");

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(content);
        }
    }
}
