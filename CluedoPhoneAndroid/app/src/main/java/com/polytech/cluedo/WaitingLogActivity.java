package com.polytech.cluedo;

/**
 * Created by Alexia on 15/01/2015.
 */
import android.app.Activity;
import android.graphics.Color;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

public class WaitingLogActivity extends Activity {
    private TextView pseudo_editText;
    private TextView perso_editText;
    private ImageView profil_picture;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_waiting_log);

        // FindView
        pseudo_editText = (TextView) findViewById(R.id.pseudoText);
        perso_editText = (TextView) findViewById(R.id.persoText);
        profil_picture = (ImageView) findViewById(R.id.imageView1);

        pseudo_editText.setText(Remote.mon_pseudo);
        perso_editText.setText("personnage : "+Remote.mon_perso);
        profil_picture.setImageResource((getResources().getIdentifier("profil_"+Remote.mon_perso.toLowerCase(), "drawable", getPackageName())));

        LinearLayout layoutActivity = (LinearLayout) findViewById(R.id.headActivity);
        String perso_temp = Remote.mon_perso.toLowerCase();
        if (perso_temp.equals("leblanc")){
            layoutActivity.setBackgroundColor(Color.parseColor("#FFFFFF"));
        } else if (perso_temp.equals("moutarde")){
            layoutActivity.setBackgroundColor(Color.parseColor("#FFFF00"));
        } else if (perso_temp.equals("olive")){
            layoutActivity.setBackgroundColor(Color.parseColor("#00FF00"));
        } else if (perso_temp.equals("pervenche")){
            layoutActivity.setBackgroundColor(Color.parseColor("#0000FF"));
        } else if (perso_temp.equals("rose")){
            layoutActivity.setBackgroundColor(Color.parseColor("#FF00FF"));
        } else { //violet
            layoutActivity.setBackgroundColor(Color.parseColor("#7F00FF"));
        }

    }

    @Override
    public void onBackPressed() {
        // Do Nothing
    }

}
