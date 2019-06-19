package com.kosta.kosta06dubbleside;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.media.Image;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    Button btnResult;
    ImageView imageView[] = new ImageView[9];
    Integer imageID[] = {R.id.iv1, R.id.iv2, R.id.iv3, R.id.iv4, R.id.iv5, R.id.iv6, R.id.iv7, R.id.iv8, R.id.iv9};

    @Override
    protected void onStart()
    {
        super.onStart();
        Log.i("메인 액티비티 테스트", "onStart()");
    }

    @Override
    protected void onStop()
    {
        super.onStop();
        Log.i("메인 액티비티 테스트", "onStop()");
    }

    @Override
    protected void onDestroy()
    {
        super.onDestroy();
        Log.i("메인 액티비티 테스트", "onDestroy()");
    }

    @Override
    protected void onPause()
    {
        super.onPause();
        Log.i("메인 액티비티 테스트", "onPause()");
    }

    @Override
    protected void onResume()
    {
        super.onResume();
        Log.i("메인 액티비티 테스트", "onResume()");
    }

    @Override
    protected void onRestart()
    {
        super.onRestart();
        Log.i("메인 액티비티 테스트", "onRestart()");
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        final String imageName[] = {
                this.getString(R.string.imageName1), this.getString(R.string.imageName2), this.getString(R.string.imageName3),
                this.getString(R.string.imageName4), this.getString(R.string.imageName5), this.getString(R.string.imageName6),
                this.getString(R.string.imageName7), this.getString(R.string.imageName8), this.getString(R.string.imageName9)
        };
        btnResult = (Button)findViewById(R.id.btnResult);

        final Integer voteCount[] = new Integer[9];
        for(int i=0; i<voteCount.length; i++)
            voteCount[i] = 0;

        for(int i=0; i<imageID.length; i++)
        {
            final  int index = i;
            imageView[index] = (ImageView)findViewById(imageID[i]);
            imageView[index].setOnClickListener(new View.OnClickListener()
            {
                @Override
                public void onClick(View view)
                {
                    voteCount[index]++;
                    Toast.makeText(getApplicationContext(), imageName[index] + " : 총" + voteCount[index] + "표", Toast.LENGTH_SHORT).show();
                }
            });
        }

        btnResult.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent mIntent = new Intent(MainActivity.this, ResultActivity.class);
                mIntent.putExtra("VoteCount", voteCount);
                mIntent.putExtra("ImageName", imageName);
                startActivity(mIntent);
            }
        });

        Log.i("메인 액티비티 테스트", "OnCreate()");
    }
}
