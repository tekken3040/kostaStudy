package com.kosta.kosta07;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    Button btnNewActivity;
    EditText editNum1, editNum2;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btnNewActivity = (Button)findViewById(R.id.btnNewActivity);
        editNum1 = (EditText)findViewById(R.id.editNum1);
        editNum2 = (EditText)findViewById(R.id.editNum2);

        final Intent mIntent = new Intent(MainActivity.this, SubActivity.class);
        mIntent.putExtra("NUM1", Integer.parseInt(editNum1.getText().toString()));
        mIntent.putExtra("NUM2", Integer.parseInt(editNum2.getText().toString()));

        btnNewActivity.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                startActivityForResult(mIntent, 0);
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode,int resultCode,@Nullable Intent data)
    {
        if(resultCode == RESULT_OK)
        {
            int hap = data.getIntExtra("HAP", 0);
            Toast.makeText(getApplicationContext(), "HAP : " + hap, Toast.LENGTH_SHORT).show();
        }
        else
        {
            Toast.makeText(getApplicationContext(), "예외", Toast.LENGTH_SHORT).show();
        }
    }
}
