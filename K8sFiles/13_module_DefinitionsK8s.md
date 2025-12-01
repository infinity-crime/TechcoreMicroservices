Концепции K8s.

Pod - минимальная еденица развертывания в Kubernetes, которая может содержать один или несколько контейнеров. Pod обеспечивает изоляцию и управление ресурсами для контейнеров внутри него.
Pod является короткоживущим объектом, который может быть создан, удален и пересоздан в зависимости от состояния кластера и требований приложения.
Он создается путем манифеста YAML, который определяет конфигурацию Pod, включая контейнеры, объемы, сетевые настройки и другие параметры.
Минимальный пример:
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: my-pod
  spec:
  containers:
	- name: my-container
	  image: nginx
```

Deployment - объект высокого уровня в Kubernetes, который управляет созданием и обновлением Pod'ов. 
Обеспечивает декларативное управление состоянием приложения, позволяя легко масштабировать, обновлять и откатывать версии приложений. 
Он необходим, чтобы явно не создавать и не управлять Pod'ами вручную.
Описывается с помощью манифеста YAML, который определяет желаемое состояние приложения, включая количество реплик, стратегию обновления и шаблон Pod'ов.
Минимальный пример:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: book-service
spec:
  replicas: 2
  selector:
    matchLabels:
      app: book-service
  template:
    metadata:
      labels:
        app: book-service
    spec:
      containers:
      - name: book
        image: registry/book-service:latest
        ports:
        - containerPort: 8081
```

Service - абстракция в Kubernetes, которая определяет логический набор Pod'ов и политику доступа к ним.
Реализует постоянный IP-адрес и DNS-имя для набора Pod'ов, обеспечивая стабильный доступ к приложениям, даже если Pod'ы создаются и удаляются динамически.
Service описывается с помощью манифеста YAML, который определяет тип сервиса (ClusterIP, NodePort, LoadBalancer и т.д.), селекторы для выбора Pod'ов и порты для доступа.
Минимальный пример:
```yaml
apiVersion: v1
kind: Service
metadata:
  name: book-service
spec:
  selector:
    app: book-service
  ports:
  - port: 80
    targetPort: 8081
  type: ClusterIP
```

ConfigMap - объект в Kubernetes, который используется для хранения конфигурационных данных в виде пар ключ-значение.
ConfigMap позволяет отделить конфигурацию приложения от его кода, что облегчает управление и обновление настроек без необходимости пересборки контейнеров.
Создается с помощью манифеста YAML, который определяет ключи и значения конфигурации.
Минимальный пример:
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: book-config
data:
  LOG_LEVEL: "Debug"
  FEATURE_X_ENABLED: "true"
```

Secret - объект в Kubernetes, предназначенный для хранения конфиденциальной информации, такой как пароли, токены и ключи.
Secret обеспечивает безопасное хранение и управление чувствительными данными, предотвращая их утечку в открытом виде.
Создается с помощью манифеста YAML, который определяет ключи и значения, обычно в закодированном виде (Base64).
Минимальный пример:
```bash
kubectl create secret generic postgres-secret \
  --from-literal=POSTGRES_USER=postgres \
  --from-literal=POSTGRES_PASSWORD=554034
```

PersistentVolumeClaim (PVC) - объект в Kubernetes, который запрашивает и управляет постоянным хранилищем для Pod'ов.
PVC позволяет приложениям запрашивать определенный объем и тип хранилища без необходимости знать детали его реализации.
Создается с помощью манифеста YAML, который определяет требования к хранилищу, такие как размер и доступность.
Минимальный пример:
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: postgres-pvc
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 5Gi
  storageClassName: standard
```