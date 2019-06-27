package com.example.kosta31sql_lite;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.os.strictmode.SqliteObjectLeakedViolation;
import android.util.Log;

public class DBHelper extends SQLiteOpenHelper {

    public DBHelper(Context context) {
        super(context, "hhj.db", null, 1);
    }

    @Override
    public void onCreate(SQLiteDatabase sqLiteDatabase) {
        Log.d("테스트", "데이터베이스가 생성되었습니다.");
        String sql = "CREATE TABLE TestTable ( " +
                "idx INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "textData TEXT NOT NULL, " +
                "intData INTEGER NOT NULL, " +
                "floatData REAL NOT NULL,  "+
                "dateData date " +
                ");";
        sqLiteDatabase.execSQL(sql);
    }

    @Override
    public void onUpgrade(SQLiteDatabase sqLiteDatabase,
            int oldVersion, int newVersion) {
        switch (oldVersion){
        case 1:
        case 2:
        case 3:
        }
        Log.d("테스트 ", "old :" + oldVersion);
        Log.d("테스트 ", "new :" + newVersion);
    }
}
