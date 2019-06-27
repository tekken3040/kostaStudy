package com.kosta.lec08sqlite;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

public class MainActivity extends AppCompatActivity {

   TextView textView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        textView = (TextView)findViewById(R.id.textView);
    }
    public void btnInsertMethod(View view){
        DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase db = dbHelper.getWritableDatabase();
        String sql = "insert into TestTable " +
         "(textData, intData,floatData, dateData) values(?,?,?,?)";
        SimpleDateFormat sdf =
                new SimpleDateFormat("yyyy-mm-dd",
                        Locale.getDefault());
        String date = sdf.format(new Date());
        String[] arg1= {"문자열1","100", "11.11",date };
        String[] arg2= {"문자열2","200", "22.22",date };
        db.execSQL(sql,arg1);
        db.execSQL(sql,arg2);
        db.close();
        textView.setText("저장완료");
    }
}
