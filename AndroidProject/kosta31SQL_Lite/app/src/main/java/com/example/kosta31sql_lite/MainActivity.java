package com.example.kosta31sql_lite;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
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

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        textView = (TextView)findViewById(R.id.textView);
    }

    public void btnInsertMethod(View view)
    {
        com.example.kosta31sql_lite.DBHelper dbHelper = new com.example.kosta31sql_lite.DBHelper(this);
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

    public void btnSelectMethod(View view)
    {
        com.example.kosta31sql_lite.DBHelper dbHelper = new com.example.kosta31sql_lite.DBHelper(this);
        SQLiteDatabase db = dbHelper.getWritableDatabase();
        String sql = "select * from TestTable order by idx desc limit 3 ";
        Cursor cursor = db.rawQuery(sql, null);
        textView.setText("");
        while(cursor.moveToNext())
        {
            int idx_pos = cursor.getColumnIndex("idx");
            int textData_pos = cursor.getColumnIndex("textData");
            int intData_pos = cursor.getColumnIndex("intData");
            int floatData_pos = cursor.getColumnIndex("floatData");
            int dateData_pos = cursor.getColumnIndex("dateData");

            int idx = cursor.getInt(idx_pos);
            String textData = cursor.getString(textData_pos);
            int intData = cursor.getInt(intData_pos);
            double floatData = cursor.getFloat(floatData_pos);
            String dateData = cursor.getString(dateData_pos);

            textView.append("idx : " + idx + "\n");
            textView.append("textData : " + textData + "\n");
            textView.append("intData : " + intData + "\n");
            textView.append("floatData : " + floatData + "\n");
            textView.append("dateData : " + dateData + "\n");
        }

        db.close();
    }

    public void btnUpdateMethod(View v)
    {
        DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase db = dbHelper.getWritableDatabase();
        String sql = "update TestTable set textData =? where idx = ?";
        String[] args = {"수정", "23"};
        db.execSQL(sql, args);
        db.close();
        textView.setText("수정완료");
    }

    public void btnDeleteMethod(View v)
    {
        DBHelper dbHelper = new DBHelper(this);
        SQLiteDatabase db = dbHelper.getWritableDatabase();
        String sql = "delete from TestTable where idx = ?";
        String[] args = {"26"};
        db.execSQL(sql, args);
        db.close();
        textView.setText("삭제완료");
    }
}
