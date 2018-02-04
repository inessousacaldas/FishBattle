package com.cilugame.h1.activity;

import android.app.AlertDialog;
import android.app.Dialog;
import android.app.DialogFragment;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.cilugame.android.commons.IOUtils;
import com.cilugame.h1.patch.ApkDownloader;
import com.cilugame.h1.patch.IPackageDownloadListener;

import java.io.File;
import java.io.IOException;
import java.util.concurrent.TimeUnit;

public class CLGamePacketFragment extends DialogFragment {

    public static final int STATE_PROCESS_STOP = 0;

    public static final int STATE_PROCESS_UPDATING = 1;

    public static final int STATE_PROCESS_CAN_INSTALL = 2;

    private int btnProcessState = STATE_PROCESS_STOP;

    private static final double secondInNano = Double.valueOf(TimeUnit.SECONDS.toNanos(1));

    private Button btnProcess;

    private TextView txtUpdateTitle;

    private TextView txtSizeTitle;
    private TextView txtSpeedView;
    private TextView txtRestTimeView;

    private ProgressBar progressBar;

    private ApkDownloader downloader;

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        final String appPackageName = getActivity().getApplicationContext().getPackageName();
        final int rPacketLayoutId = getResources().getIdentifier("fragment_clgame_packet", "layout", appPackageName);

        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        LayoutInflater inflater = getActivity().getLayoutInflater();
        View view = inflater.inflate(rPacketLayoutId, null);
        builder.setView(view);
        createDialogTitle(builder);

        final int rTxtUpdateContentId = getResources().getIdentifier("cilu_txt_packet_update_content", "id", appPackageName);
        final int rTxtUpdateTitleId = getResources().getIdentifier("cilu_txt_packet_update_title", "id", appPackageName);
        final int rProgressBarId = getResources().getIdentifier("cilu_game_packet_pbLoader", "id", appPackageName);
        final int rTxtSizeTitleId = getResources().getIdentifier("cilu_txt_packet_size", "id", appPackageName);
        final int rBtnProcessId = getResources().getIdentifier("cilu_btn_packet_process", "id", appPackageName);
        final int rTxtSpeedId = getResources().getIdentifier("cilu_txt_packet_speed", "id", appPackageName);
        final int rTxtRestTimeId = getResources().getIdentifier("cilu_txt_packet_rest_time", "id", appPackageName);

        TextView txtUpdateContent = (TextView) view.findViewById(rTxtUpdateContentId);
        progressBar = (ProgressBar) view.findViewById(rProgressBarId);
        progressBar.setProgress(0);
        txtUpdateTitle = (TextView) view.findViewById(rTxtUpdateTitleId);
        txtSizeTitle = (TextView) view.findViewById(rTxtSizeTitleId);
        txtSpeedView = (TextView) view.findViewById(rTxtSpeedId);
        txtRestTimeView = (TextView) view.findViewById(rTxtRestTimeId);

        txtUpdateTitle.setText(downloader.getUpdateTitle());
        txtUpdateContent.setText(downloader.getUpdateMessage());

        btnProcess = (Button) view.findViewById(rBtnProcessId);
        if (downloader.isTaskRunning()) {
            btnProcessState = STATE_PROCESS_UPDATING;
            btnProcess.setText("暂停");
            txtSpeedView.setText("");
            txtRestTimeView.setText("");
        }
        btnProcess.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (btnProcessState == STATE_PROCESS_UPDATING) {
                    stop();
                } else if (btnProcessState == STATE_PROCESS_STOP) {
                    updating();
                } else if (btnProcessState == STATE_PROCESS_CAN_INSTALL) {
                    install();
                }
            }
        });

        File outputFile = downloader.getOutputFile();
        long progress = outputFile.exists() ? outputFile.length() : 0;
        progressUpdate(progress, downloader.getContentLength());
        if (progress > 0 && btnProcessState == STATE_PROCESS_STOP) {
            btnProcess.setText("继续更新");
        }
        if (progressBar.getProgress() >= progressBar.getMax()) {
            downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_DOWNLOAD_FINISH);
            canInstall();
        }
        this.setCancelable(false);
        builder.setCancelable(false);
        Dialog dialog = builder.create();
        downloader.setDialog(this);
        return dialog;
    }

    public void createDialogTitle(AlertDialog.Builder builder) {

        RelativeLayout rootLayout = new RelativeLayout(this.getActivity());
        RelativeLayout.LayoutParams paramsrl = new RelativeLayout.LayoutParams(LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT);
        rootLayout.setLayoutParams(paramsrl);
        rootLayout.setBackgroundColor(Color.DKGRAY);

        TextView title = new TextView(getActivity());
        title.setText("游戏更新");
        title.setBackgroundColor(Color.DKGRAY);
        title.setPadding(10, 10, 10, 10);
        title.setGravity(Gravity.CENTER);
        title.setTextColor(Color.WHITE);
        title.setTextSize(20);

        RelativeLayout.LayoutParams titleLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        titleLayoutParams.addRule(RelativeLayout.CENTER_IN_PARENT);
        rootLayout.addView(title, titleLayoutParams);

        Drawable closeDrawable = null;
        try {
            closeDrawable = Drawable.createFromStream(getActivity().getAssets().open("cilu_dialog_close.png"), null);
        } catch (IOException e) {
            Log.w("CLGamePacketFragment", "get close button png error:", e);
        }
        if (closeDrawable != null) {
            ImageView closeImage = new ImageView(this.getActivity());
            closeImage.setImageDrawable(closeDrawable);
            closeImage.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    preCloseConfirm();
                }
            });
            closeImage.setPadding(0, 10, 20, 10);

            int px = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 40, getResources().getDisplayMetrics());
            closeImage.setAdjustViewBounds(true);
            closeImage.setMaxWidth(px);
            closeImage.setMaxHeight(px);


            RelativeLayout.LayoutParams closeImageLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
            closeImageLayoutParams.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);
            rootLayout.addView(closeImage, closeImageLayoutParams);
        }

        builder.setCustomTitle(rootLayout);
    }

    public void preCloseConfirm() {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        final String exitButtonTitle = downloader.isExitGame() ? "退出游戏" : "退出更新";
        builder.setTitle("提示");
        builder.setMessage("更新未完成, 您定退出更新吗?");
        builder.setPositiveButton("继续更新", null);
        final CLGamePacketFragment self = this;
        builder.setNegativeButton(exitButtonTitle, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                downloader.pause();
                downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_QUIT_DOWNLOAD);
                Dialog d = self.getDialog();
                if (d != null && d.isShowing()) {
                    if(downloader.isExitGame()) {
                        getActivity().finish();
                    } else {
                        d.dismiss();
                    }
                }
            }
        });
        builder.create().show();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (downloader == null) {
            downloader = ApkDownloader.getCurrent();
        }
    }

    private void stop() {
        btnProcessState = STATE_PROCESS_STOP;
        btnProcess.setText("继续更新");
        txtSpeedView.setText("");
        txtRestTimeView.setText("");
        downloader.pause();
        downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_STOP);
    }

    public void updating() {
        btnProcessState = STATE_PROCESS_UPDATING;
        btnProcess.setText("暂停");
        downloader.start();
        downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_START);
    }

    public void canInstall() {
        btnProcessState = STATE_PROCESS_CAN_INSTALL;
        btnProcess.setText("安装");
    }

    public void progressUpdate(Long... progress) {
        // check if has error!
        if (progress.length > 2 && 2 == progress[2]) {
            onDownloadError();
            return;
        }
        // if we get here, length is known, now set indeterminate to false
        progressBar.setIndeterminate(false);
        int percent = (int) (progress[0].floatValue() / progress[1].floatValue() * 100);
        progressBar.setProgress(percent);

        double speedInByte = progress.length > 3 ? progress[3] : 0L;
        if (speedInByte>0) {
            txtSpeedView.setText("(" + IOUtils.byteCountToDisplaySize(speedInByte) + "/s)");
            int restSeconds = (int) Math.ceil((downloader.getContentLength() - progress[0]) / speedInByte);
            txtRestTimeView.setText(toHumanTimeFormat(restSeconds));
        }

        String titleText = IOUtils.byteCountToDisplaySize(downloader.getContentLength()) + "";
        if (percent > 0) {
            titleText = IOUtils.byteCountToDisplaySize(progress[0]) + "/" + IOUtils.byteCountToDisplaySize(downloader.getContentLength());
        }
        if (percent >= 100) {
            txtSpeedView.setText("(下载完成)");
            txtRestTimeView.setText("");
        }
        txtSizeTitle.setText(titleText);

        if (progress.length > 2 && 1 == progress[2] && percent >= 100) {
            downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_DOWNLOAD_FINISH);
            install();
        }
    }

    private void onDownloadError() {
        stop();
        if (getDialog() != null && getDialog().isShowing()) {
            AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
            builder.setTitle("网络异常");
            builder.setMessage("当前网络不可用,无法更新文件内容,请检查网络设置");
            builder.setNegativeButton("确定", null);
            builder.create().show();
        }
    }

    public String toHumanTimeFormat(int totalSeconds) {
        if (totalSeconds < 0) {
            return "00:00:00";
        }
        int seconds = (totalSeconds / 1) % 60;
        int minutes = ((totalSeconds / (1 * 60)) % 60);
        int hours = ((totalSeconds / (1 * 60 * 60)) % 24);
        if (hours > 99) {
            return "99:59:59";
        }
        return String.format("%02d:%02d:%02d", hours, minutes, seconds);
    }

    public void install() {
        if (getDialog() != null && getDialog().isShowing()) {
            downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_INSTALL);
            Intent promptInstall = new Intent(Intent.ACTION_VIEW)
                    .setDataAndType(Uri.fromFile(downloader.getOutputFile()),
                            "application/vnd.android.package-archive");
            startActivityForResult(promptInstall, 3000);
        } else {
            Log.d("packet_dialog", "dialog not showing, skill install!");
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        System.out.println("CLGamePacket - onActivityResult" + requestCode + ", " + resultCode + ", " + data);
        if (requestCode == 3000) {
            canInstall();
            downloader.publishEvent(IPackageDownloadListener.EVENT_TYPE_QUIT_INSTALL);
        }
    }

    public ApkDownloader getDownloader() {
        return downloader;
    }

    public void setDownloader(ApkDownloader downloader) {
        this.downloader = downloader;
    }
}
