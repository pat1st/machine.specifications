﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Selenium;

namespace Machine.Specifications.SeleniumSupport
{
  public abstract class SeleniumResultSupplementer : ISupplementSpecificationResults
  {
    protected abstract string ImagesPath { get; }
    protected abstract DefaultSelenium Selenium { get; }

    public Result SupplementResult(Result result)
    {
      if (result.Passed || result.Status == Status.Ignored)
        return result;

      Guid guid = Guid.NewGuid();
      string pageScreenshotPath = Path.Combine(ImagesPath, guid.ToString() + "-full-page-screenshot.png");
      string screenshotPath = Path.Combine(ImagesPath, guid.ToString() + "-screenshot.png");
      string htmlPath = Path.Combine(ImagesPath, guid.ToString() + ".html");

      Dictionary<string, string> supplement = new Dictionary<string, string>();

      try
      {
        Selenium.CaptureEntirePageScreenshot(pageScreenshotPath, "");
        supplement["img-full-page-screenshot"] = pageScreenshotPath;
      }
      catch (Exception err)
      {
        Console.Error.WriteLine("Problem capturing Selenium img-full-page-screenshot");
        Console.Error.WriteLine(err);
      }

      try
      {
        Selenium.CaptureScreenshot(screenshotPath);
        supplement["img-screenshot"] = screenshotPath;
      }
      catch (Exception err)
      {
        Console.Error.WriteLine("Problem capturing Selenium img-screenshot");
        Console.Error.WriteLine(err);
      }

      try
      {
        supplement["text-log"] = Selenium.RetrieveLastRemoteControlLogs();
      }
      catch (Exception err)
      {
        Console.Error.WriteLine("Problem capturing Selenium text-log");
        Console.Error.WriteLine(err);
      }

      try
      {
        using (var writer = new StreamWriter(htmlPath))
        {
          writer.Write(Selenium.GetHtmlSource());
        }
        supplement["html-source"] = htmlPath;  
      }
      catch (Exception err)
      {
        Console.Error.WriteLine("Problem capturing Selenium html-source");
        Console.Error.WriteLine(err);
      }

      return Result.Supplement(result, "Selenium", supplement);
    }
  }
}