package com.example.kosta34buspath;

import androidx.appcompat.app.AppCompatActivity;

import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.TextView;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import javax.net.ssl.HttpsURLConnection;

public class MainActivity extends AppCompatActivity
{
    TextView txtBus, txtData;
    String getData;
    int busNumber = 0;

    String strSrch = busNumber + "";
    int count;

    String _key = "NZpqrtDX%2Fvbri16sFn2EhVlgpeRA60zNDD1ATUj6I43wrx4LFbHDVU%2Bo4%2FC2Ku5m9Pv42o%2Fk6QCFubJhWfBlpQ%3D%3D";
    StringBuilder stringBuilder;
    int _busNum = 0;
    String strUrl = null;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        stringBuilder = new StringBuilder();
        txtBus = (TextView)findViewById(R.id.txtBus);
        txtData = (TextView)findViewById(R.id.txtData);
        stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=");
        strUrl = stringBuilder.toString();

        //String serviceUrl = "http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?";
        //String strRUL = serviceUrl + "serviceKey=" + _key + "&strSrch=" + strSrch;
        _busNum = 3;
        DownloadWebContent downloadWebContent = new DownloadWebContent();
        downloadWebContent.execute(strUrl + _busNum);
    }

    public class DownloadWebContent extends AsyncTask<String, Void, String>
    {
        @Override
        protected String doInBackground(String... strings)
        {
            try
            {
                return dounloadByUrl((String)strings[0]);
            }
            catch(IOException e)
            {
                e.printStackTrace();
            }
            return "다운로드 실패";
        }

        @Override
        protected void onPostExecute(String result)
        {
            //super.onPostExecute(result);
            String headerCd = "";
            String busRouteId = "";
            boolean is_headerCd = false;
            boolean is_busRouteId = false;

            try
            {
                XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
                factory.setNamespaceAware(true);
                XmlPullParser xmlPullParser = factory.newPullParser();
                xmlPullParser.setInput(new StringReader(result));
                int eventType = xmlPullParser.getEventType();
                while(eventType != XmlPullParser.END_DOCUMENT)
                {
                    if(eventType == XmlPullParser.START_DOCUMENT)
                    {

                    }
                    else if(eventType == XmlPullParser.START_TAG)
                    {
                        String tag_name = xmlPullParser.getName();
                        if(tag_name.equals("headerCd"))
                            is_headerCd = true;
                        if(tag_name.equals("busRouteId"))
                            is_busRouteId = true;
                    }
                    else if(eventType == XmlPullParser.TEXT)
                    {
                        if(is_headerCd)
                        {
                            headerCd=xmlPullParser.getText();
                            is_headerCd = false;
                        }
                        if(headerCd.equals("0"))
                        {
                            if(is_busRouteId)
                            {
                                busRouteId=xmlPullParser.getText();
                                is_busRouteId=false;
                            }
                        }
                    }
                    else if(eventType == XmlPullParser.END_TAG)
                    {

                    }
                    eventType = xmlPullParser.next();
                }
            }
            catch(XmlPullParserException|IOException e)
            {
                txtData.setText(e.getMessage());
                e.printStackTrace();
            }

            stringBuilder.delete(0, stringBuilder.length());
            stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getStaionByRoute?ServiceKey=").append(_key).append("&busRouteId=").append(busRouteId);

            DownloadWebContent2 downloadWebContent2 = new DownloadWebContent2();
            downloadWebContent2.execute(stringBuilder.toString());
            //txtBus.setText(result);
        }

        public String dounloadByUrl(String inUrl) throws IOException
        {
            HttpURLConnection connection = null;
            try
            {
                URL url=new URL(inUrl);
                connection=(HttpURLConnection)url.openConnection();
                BufferedInputStream bufferedInputStream=new BufferedInputStream(connection.getInputStream());
                BufferedReader bufferedReader=new BufferedReader(new InputStreamReader(bufferedInputStream,"utf-8"));
                String line=null;
                getData="";
                while((line=bufferedReader.readLine())!=null)
                {
                    getData+=line;
                }

                return getData;
            }
            finally
            {
                connection.disconnect();
            }
        }
    }

    public void btnResetMethod(View view)
    {
        stringBuilder.delete(0, stringBuilder.length());
        stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=").append(_busNum);

        DownloadWebContent downloadWebContent = new DownloadWebContent();
        downloadWebContent.execute(stringBuilder.toString());

        txtData.setText("");
        txtBus.setText("");
        txtBus.append("버스번호 : ");
        txtBus.append(_busNum+"\n");
    }

    public void plusMethod(View view)
    {
        _busNum++;
        stringBuilder.delete(0, stringBuilder.length());
        stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=").append(_busNum);

        DownloadWebContent downloadWebContent = new DownloadWebContent();
        downloadWebContent.execute(stringBuilder.toString());

        txtData.setText("");
        txtBus.setText("");
        txtBus.append("버스번호 : ");
        txtBus.append(_busNum+"\n");
    }

    public void plus100Method(View view)
    {
        _busNum += 100;
        stringBuilder.delete(0, stringBuilder.length());
        stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=").append(_busNum);

        DownloadWebContent downloadWebContent = new DownloadWebContent();
        downloadWebContent.execute(stringBuilder.toString());

        txtData.setText("");
        txtBus.setText("");
        txtBus.append("버스번호 : ");
        txtBus.append(_busNum+"\n");
    }

    public void minusMethod(View view)
    {
        _busNum--;
        stringBuilder.delete(0, stringBuilder.length());
        stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=").append(_busNum);

        DownloadWebContent downloadWebContent = new DownloadWebContent();
        downloadWebContent.execute(stringBuilder.toString());

        txtData.setText("");
        txtBus.setText("");
        txtBus.append("버스번호 : ");
        txtBus.append(_busNum+"\n");
    }

    public void minus100Method(View view)
    {
        _busNum -= 100;
        stringBuilder.delete(0, stringBuilder.length());
        stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=").append(_busNum);

        DownloadWebContent downloadWebContent = new DownloadWebContent();
        downloadWebContent.execute(stringBuilder.toString());

        txtData.setText("");
        txtBus.setText("");
        txtBus.append("버스번호 : ");
        txtBus.append(_busNum+"\n");
    }

    public class DownloadWebContent2 extends AsyncTask<String, Void, String>
    {
        @Override
        protected String doInBackground(String... strings)
        {
            try
            {
                return dounloadByUrl((String)strings[0]);
            }
            catch(IOException e)
            {
                e.printStackTrace();
            }
            return "다운로드 실패";
        }

        @Override
        protected void onPostExecute(String result)
        {
            //super.onPostExecute(result);
            String headerCd = "";
            String gpsX = "",  gpsY = "", stationNm = "", sectSpd = "", direction = "";
            boolean is_headerCd = false;
            boolean is_gpsX = false;
            boolean is_gpsY = false;
            boolean is_stationNm = false;
            boolean is_sectSpd = false;
            boolean is_direction = false;

            txtBus.append("-버스 위치 검색 결과-\n");
            try
            {
                XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
                factory.setNamespaceAware(true);
                XmlPullParser xmlPullParser = factory.newPullParser();
                xmlPullParser.setInput(new StringReader(result));
                int eventType = xmlPullParser.getEventType();
                count = 0;
                while(eventType != XmlPullParser.END_DOCUMENT)
                {
                    if(eventType == XmlPullParser.START_DOCUMENT)
                    {

                    }
                    else if(eventType == XmlPullParser.START_TAG)
                    {
                        String tag_name = xmlPullParser.getName();
                        switch(tag_name)
                        {
                        case "headerCd":
                            is_headerCd = true;
                            break;

                        case "gpsX":
                            is_gpsX = true;
                            break;

                        case "gpsY":
                            is_gpsY = true;
                            break;

                        case "stationNm":
                            is_stationNm = true;
                            break;

                        case "sectSpd":
                            is_sectSpd = true;
                            break;

                        case "direction":
                            is_direction = true;
                            break;
                        }
                    }
                    else if(eventType == XmlPullParser.TEXT)
                    {
                        if(is_headerCd)
                        {
                            headerCd=xmlPullParser.getText();
                            is_headerCd = false;
                        }
                        if(headerCd.equals("0"))
                        {
                            if(is_gpsX)
                            {
                                count++;
                                txtBus.append("=============================================\n");
                                gpsX = xmlPullParser.getText();
                                txtBus.append("("+count+") gpsX : "+gpsX+"\n");
                                is_gpsX = false;
                            }
                            if(is_gpsY)
                            {
                                gpsY = xmlPullParser.getText();
                                txtBus.append("("+count+") gpsY : "+gpsY+"\n");
                                is_gpsY = false;
                            }
                            if(is_stationNm)
                            {
                                stationNm = xmlPullParser.getText();
                                txtBus.append("("+count+") 정류장 이름 : "+stationNm+"\n");
                                is_stationNm = false;
                            }
                            if(is_direction)
                            {
                                direction = xmlPullParser.getText();
                                txtBus.append("("+count+") 진행 방향 : "+direction+"\n");
                                is_direction = false;
                            }
                            if(is_sectSpd)
                            {
                                sectSpd = xmlPullParser.getText();
                                txtBus.append("("+count+") 구간 속도 : "+is_sectSpd+"\n");
                                is_sectSpd = false;
                            }
                        }
                    }
                    else if(eventType == XmlPullParser.END_TAG)
                    {

                    }
                    eventType = xmlPullParser.next();
                }
            }
            catch(XmlPullParserException|IOException e)
            {
                txtData.setText(e.getMessage());
                e.printStackTrace();
            }
            //stringBuilder.append("http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?ServiceKey=").append(_key).append("&strSrch=");
            //http://ws.bus.go.kr/api/rest/busRouteInfo/getStaionByRoute?ServiceKey=인증키&busRouteId=100100112


            //txtData.setText(result);
        }

        public String dounloadByUrl(String inUrl) throws IOException
        {
            HttpURLConnection connection = null;
            try
            {
                URL url=new URL(inUrl);
                connection=(HttpURLConnection)url.openConnection();
                BufferedInputStream bufferedInputStream=new BufferedInputStream(connection.getInputStream());
                BufferedReader bufferedReader=new BufferedReader(new InputStreamReader(bufferedInputStream,"utf-8"));
                String line=null;
                getData="";
                while((line=bufferedReader.readLine())!=null)
                {
                    getData+=line;
                }

                return getData;
            }
            finally
            {
                connection.disconnect();
            }
        }
    }
}
