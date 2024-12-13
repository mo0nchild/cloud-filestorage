package com.example.androidclient.pages

import android.widget.Toast
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.material3.Button
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import coil.compose.rememberAsyncImagePainter
import coil.compose.rememberImagePainter

@Composable
fun ImageGridPage(navController: NavController) {
    var images by remember { mutableStateOf(listOf<String>()) } // Список изображений
    val context = LocalContext.current

    // Создание ActivityResultLauncher для выбора изображения из галереи
    val getImage = rememberLauncherForActivityResult(ActivityResultContracts.GetContent()) { uri ->
        uri?.let {
            images = images + it.toString()  // Добавляем выбранный путь изображения в список
        }
    }

    Column(modifier = Modifier
        .fillMaxSize()
        .background(Color(0xFFF5F5F5))
        .padding(16.dp)) {
        // Кнопка добавления изображения
        Button(
            onClick = {
                getImage.launch("image/png") // Запускаем выбор изображения
            },
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp)
        ) {
            Text("Add Image")
        }

        // Сетка изображений
        LazyVerticalGrid(
            columns = GridCells.Fixed(3), // Количество столбцов
            modifier = Modifier
                .fillMaxSize()
                .padding(8.dp)
        ) {
            items(images.size) { index ->
                ImageItem(imagePath = images[index])
            }
        }
    }
}

@Composable
fun ImageItem(imagePath: String) {
    Image(
        painter = rememberAsyncImagePainter(imagePath),  // Загружаем изображение по URI
        contentDescription = null,
        modifier = Modifier
            .padding(8.dp)
            .fillMaxWidth()
            .aspectRatio(1f) // Сохраняем квадратную форму
            .background(Color.Gray) // Фон для изображения
    )
}