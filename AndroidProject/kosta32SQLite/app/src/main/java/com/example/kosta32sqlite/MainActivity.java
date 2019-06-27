package com.example.kosta32sqlite;

import androidx.appcompat.app.AppCompatActivity;

import android.content.ContentValues;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

public class MainActivity extends AppCompatActivity
{
    TextView textView;
    DBHelper dbHelper;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        textView = (TextView)findViewById(R.id.textView);
        dbHelper = new DBHelper(this);
    }

    public void btnInsertMethod(View view)
    {
        //DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-mm-dd",Locale.getDefault());
        String date = simpleDateFormat.format(new Date());

        ContentValues contentValues = new ContentValues();
        contentValues.put("textData", "문자열1");
        contentValues.put("intData", 100);
        contentValues.put("floatData", 3.14);
        contentValues.put("dateData", date);
        database.insert("TestTable", null, contentValues);
        database.close();
        textView.setText("저장완료");
    }

    public void btnSelectMethod(View view)
    {
        //DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        Cursor cursor = database.query("TestTable", null, null, null, null, null, "idx desc", "2");

        while(cursor.moveToNext())
        {
            int idx= cursor.getInt(cursor.getColumnIndex("idx"));
            String textData= cursor.getString(cursor.getColumnIndex("textData"));
            int intData= cursor.getInt(cursor.getColumnIndex("intData"));
            float floatData= cursor.getFloat(cursor.getColumnIndex("floatData"));
            String dateData= cursor.getString(cursor.getColumnIndex("dateData"));
            textView.append("idx : " + idx + "\n");
            textView.append("textData : " + textData + "\n");
            textView.append("intData : " + intData + "\n");
            textView.append("floatData : " + floatData + "\n");
            textView.append("dateData : " + dateData + "\n");
        }

        database.close();
    }

    public void btnUpdateMethod(View view)
    {
        //DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        ContentValues contentValues = new ContentValues();
        contentValues.put("textData", "서점");
        String where = "idx=?";
        String[] args = {"1"};
        database.update("TestTable", contentValues, where, args);
        database.close();
        textView.setText("업데이트 완료");
    }

    public void btnDeleteMethod(View view)
    {
        //DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        String where = "idx=?";
        String[] args = {"2"};
        database.delete("TestTable", where, args);
        database.close();
        textView.setText("삭제 완료");
    }
}
