package com.example.kosta19fragment2;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.FragmentManager;

import android.os.Bundle;

public class MainActivity extends AppCompatActivity implements ListFragment.ImageSelectionCallback
{
    ListFragment listFragment;
    ViewerFragment viewerFragment;
    int[] images = {R.drawable.lena256, R.drawable.renoir06, R.drawable.renoir07};
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        FragmentManager manager = getSupportFragmentManager();
        listFragment = (ListFragment)manager.findFragmentById(R.id.listFragment);
        viewerFragment = (ViewerFragment)manager.findFragmentById(R.id.viewerFragment);
    }

    @Override
    public void onImageSelected(int position)
    {
        viewerFragment.setImageView(images[position]);
    }
}
