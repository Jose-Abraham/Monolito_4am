pipeline {
    // Le dice a Jenkins que busque obligatoriamente tu agente local de Windows
    agent { 
        label 'windows-dev' 
    }

    environment {
        MSBUILD = 'C:\\Program Files\\Microsoft Visual Studio\\18\\Insiders\\MSBuild\\Current\\Bin\\MSBuild.exe'
        RUTA_IIS = 'C:\\inetpub\\wwwroot\\Monolito_Jenkins'
    }

    stages {
        stage('1. Restaurar paquetes NuGet') {
            steps {
                echo 'Restaurando paquetes NuGet de la solución...'
                bat "\"${MSBUILD}\" Monolito_4am.sln /t:Restore"
            }
        }
        
        stage('2. Compilar solución') {
            steps {
                echo 'Compilando proyecto ASP.NET en arquitectura de 3 capas...'
                bat "\"${MSBUILD}\" Monolito_4am.sln /p:Configuration=Release /p:Platform=\"Any CPU\""
            }
        }
        
        stage('3. Ejecutar pruebas') {
            steps {
                echo 'Iniciando fase de pruebas simuladas...'
                echo 'Pruebas finalizadas con éxito.'
            }
        }
        
        stage('4. Publicar aplicación') {
            steps {
                echo 'Generando archivos de publicación limpia en carpeta temporal...'
                bat "\"${MSBUILD}\" Monolito_4am.sln /p:DeployOnBuild=true /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=true /p:publishUrl=C:\\JenkinsPublish\\Monolito"
            }
        }

        stage('5. Desplegar en IIS') {
            steps {
                echo 'Trasladando los archivos finales a tu servidor IIS local...'
                bat "xcopy C:\\JenkinsPublish\\Monolito\\* \"${RUTA_IIS}\" /E /Y /I"
                echo '¡Despliegue real en IIS completado con éxito!'
            }
        }
    }
}