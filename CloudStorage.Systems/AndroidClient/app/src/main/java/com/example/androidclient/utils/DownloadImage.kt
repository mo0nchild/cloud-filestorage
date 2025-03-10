package com.example.androidclient.utils

import android.content.Context
import android.graphics.Bitmap
import android.graphics.drawable.BitmapDrawable
import android.os.Environment
import android.widget.Toast
import coil.ImageLoader
import coil.request.ImageRequest
import coil.request.SuccessResult
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import java.io.FileOutputStream

fun downloadImageToFileSystem(context: Context, imageUrl: String, fileName: String) {
    CoroutineScope(Dispatchers.IO).launch {
        val loader = ImageLoader(context)
        val request = ImageRequest.Builder(context)
            .data(imageUrl)
            .allowHardware(false)
            .build()
        loader.execute(request).let {
            if (it is SuccessResult) {
                val bitmap = (it.drawable as BitmapDrawable).bitmap
                saveBitmapToFile(context, bitmap, fileName)
            } else {
                withContext(Dispatchers.Main) {
                    Toast.makeText(context, "Ошибка при скачивании файла", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}
private suspend fun saveBitmapToFile(context: Context, bitmap: Bitmap, fileName: String) {
    val baseDirectory = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS)
    val directory = File(baseDirectory, "DownloadedImages").apply {
        if (!exists()) mkdirs()
    }
    val file = File(directory, fileName)
    withContext(Dispatchers.IO) {
        FileOutputStream(file).use { outputStream ->
            bitmap.compress(Bitmap.CompressFormat.PNG, 100, outputStream)
            outputStream.flush()
            outputStream.close()
        }
    }
    withContext(Dispatchers.Main) {
        Toast.makeText(context, "Изображение сохранено в ${file.absolutePath}", Toast.LENGTH_SHORT).show()
    }
}