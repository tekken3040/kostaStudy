package com.kosta.kosta06dubbleside;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RatingBar;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class ResultActivity extends AppCompatActivity
{
    Button btnReturn;
    TextView tvTop;
    RatingBar rBar[] = new RatingBar[9];
    ImageView iV[] = new ImageView[9];
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_result);

        btnReturn = (Button)findViewById(R.id.btnReturn);
        tvTop = (TextView)findViewById(R.id.textView);

        Intent mIntent = getIntent();
        int[] voteCount = mIntent.getIntArrayExtra("VoteCount");
        String[] imageName = mIntent.getStringArrayExtra("imageName");

        Integer imageID[] = {R.id.iv1, R.id.iv2, R.id.iv3, R.id.iv4, R.id.iv5, R.id.iv6, R.id.iv7, R.id.iv8, R.id.iv9};
        Integer rbID[] = {R.id.rb1, R.id.rb2, R.id.rb3, R.id.rb4, R.id.rb5, R.id.rb6, R.id.rb7, R.id.rb8, R.id.rb9};

        int maxEntry = 0;
        for(int i=1; i<voteCount.length; i++)
        {
            rBar[i] = (RatingBar)findViewById(rbID[i]);
            iV[i] = (ImageView)findViewById(imageID[i]);
            if(voteCount[maxEntry] < voteCount[i])
                maxEntry = i;
        }
        List<ImageContain> voteList = new ArrayList<ImageContain>();
        for(int i=0; i<voteCount.length; i++)
        {
            ImageContain ic = new ImageContain();
            ic.imageID = imageID[i];
            ic.rbID = rbID[i];
            ic.voteCount = voteCount[i];
            voteList.add(ic);
        }

        Collections.sort(voteList, sortByVoteCount);

        for(int i=0; i<voteCount.length; i++)
        {
            //rBar[i].setRating((float)voteList.get(i+1));
        }
    }

    private final static Comparator<ImageContain> sortByVoteCount = new Comparator<ImageContain>()
    {
        @Override
        public int compare(ImageContain imageContain,ImageContain t1)
        {
            return Integer.compare(imageContain.voteCount, t1.voteCount);
        }
    };

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
}
