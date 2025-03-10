package com.example.androidclient.utils

import android.content.Context
import com.example.androidclient.requests.ApiRoutes
import com.example.androidclient.requests.SERVER_ADDRESS
import kotlinx.serialization.Serializable
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.io.File
import java.io.FileOutputStream
import java.io.InputStream

const val CONFIGURATION_FILENAME = "config.json"
@Serializable
data class Config(val apiHost: String)

fun readConfigFromFile(context: Context): Config {
    val configFile = File(context.getExternalFilesDir(null), CONFIGURATION_FILENAME)
    return if (configFile.exists()) {
        val jsonString = configFile.bufferedReader().use { it.readText() }
        Json.decodeFromString(jsonString)
    } else {
        val defaultConfig = Config(apiHost = SERVER_ADDRESS)
        writeConfigToFile(context, defaultConfig)
        defaultConfig
    }
}
private fun writeConfigToFile(context: Context, config: Config) {
    val configFile = File(context.getExternalFilesDir(null), CONFIGURATION_FILENAME)
    val jsonString = Json.encodeToString(config)
    FileOutputStream(configFile).use { it.write(jsonString.toByteArray()) }
}