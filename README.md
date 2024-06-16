# Unity VR Racegame met ML

Dit project is een VR-racegame waarin machine learning een centrale rol speelt. In deze eenvoudige maar uitdagende racegame ligt de focus op het rijden tegen AI, die getraind zijn met Proximal Policy Optimization (PPO) en Imitative Learning. Om de ervaring nog meeslepender te maken, hebben we VR-elementen geïntegreerd, zodat je volledig kunt opgaan in de racewereld.

## Team

- **Hafid Khorta** (GitHub: `Hafidk5`)
- **Seppe Faster** (GitHub: `AnormalDuck2003`)
- **Singyu Liu** (GitHub: `golden1612`)
- **Rens Philtjens** (GitHub: `gwnrens`)

## Installatie en Setup

1. Clone de repository:

   ```sh
   git clone https://github.com/gwnrens/VR-ML-Project.git
   ```

2. Open het project in Unity.

3. Zorg ervoor dat je de benodigde VR-configuratie hebt ingesteld.

4. Start het project.

## Hoe te Spelen

- Zet je VR-headset op.
- Gebruik de controllers om door de garage (menu) te navigeren en de race te starten.
- Race tegen AI en probeer de race te winnen!

## Gebruikte Assets

We hebben de volgende assets uit de Unity Asset Store en andere plaatsen gebruikt om de visuele aspecten van ons spel te verbeteren:

1. [Cartoon Race Track Oval](https://assetstore.unity.com/packages/3d/environments/roadways/cartoon-race-track-oval-175061)
2. [Low Poly Storage Pack](https://assetstore.unity.com/packages/3d/environments/urban/low-poly-storage-pack-101732)
3. [F1_lowpoly Car](https://sketchfab.com/3d-models/f1-lowpoly-301cd3e7e0084645a14b134228d3ba16)
4. [ARCADE: Racing Car](https://assetstore.unity.com/packages/3d/vehicles/land/arcade-free-racing-car-161085)

## Machine Learning Trainingsconfiguratie

De Agent in ons spel zijn getraind met behulp van de volgende configuratie:

```yaml
default_settings: null
behaviors:
  AutoAgentFinale:
    trainer_type: ppo
    hyperparameters:
      batch_size: 64
      buffer_size: 2048
      learning_rate: 0.001
      beta: 0.0005
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 5
      shared_critic: false
      learning_rate_schedule: linear
      beta_schedule: linear
      epsilon_schedule: linear
    network_settings:
      normalize: true
      hidden_units: 127
      num_layers: 2
      vis_encode_type: simple
      memory: null
      goal_conditioning_type: hyper
      deterministic: false
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
        network_settings:
          normalize: false
          hidden_units: 128
          num_layers: 2
          vis_encode_type: simple
          memory: null
          goal_conditioning_type: hyper
          deterministic: false
    init_path: null
    keep_checkpoints: 5
    checkpoint_interval: 500000
    max_steps: 500000
    time_horizon: 64
    summary_freq: 1000
    threaded: false
    self_play: null
    behavioral_cloning: null
env_settings:
  env_path: null
  env_args: null
  base_port: 5005
  num_envs: 1
  num_areas: 1
  seed: -1
  max_lifetime_restarts: 10
  restarts_rate_limit_n: 1
  restarts_rate_limit_period_s: 60
engine_settings:
  width: 84
  height: 84
  quality_level: 5
  time_scale: 20
  target_frame_rate: -1
  capture_frame_rate: 60
  no_graphics: false
environment_parameters: null
checkpoint_settings:
  run_id: CubeAgent12
  initialize_from: null
  load_model: false
  resume: true
  force: false
  train_model: false
  inference: false
  results_dir: results
torch_settings:
  device: null
debug: false
```

## Machine Learning Grafieken

Hieronder vind je grafieken die de prestaties en leerprogressie van onze AI-agenten tijdens de training laten zien.

![Trainingsgrafiek 1](https://media.discordapp.net/attachments/1233345750755184774/1251517126913560659/image.png?ex=666edda7&is=666d8c27&hm=8878616d99ed9984dbdcf97584a6d4a8ad594ee9708e492c2ee72bbbd920f214&=&format=webp&quality=lossless&width=1307&height=597)
![Trainingsgrafiek 2](https://media.discordapp.net/attachments/1233345750755184774/1251517129249787994/image.png?ex=666edda7&is=666d8c27&hm=ed1e08eea357ef94a325bdd16289ac2751d598aa95f97144a521a7cba4fc2307&=&format=webp&quality=lossless&width=1308&height=596)
