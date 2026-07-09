# 🚀 Spyte (Space Invaders Remake)

Ei, gamer! Seja bem-vindo ao **Spyte** — uma versão fresquinha e cheia de ação do clássico jogo arcade *Space Invaders*, feita com muito carinho em Unity e C#. Aqui a missão é simples: proteger a nave, derrubar todos os aliens e bater aquele recorde! 👾✨

## 🎯 Sobre o Jogo

Nesse game, você controla uma nave tentando eliminar ondas de alienígenas que descem pela tela. Tem que ficar esperto, porque a dificuldade é dinâmica e o jogo está recheado de novidades:

- O Enxame: Os aliens estão organizados numa matriz que avança lentamente. Cuidado: barreiras de proteção estão lá para ajudar, mas quebram com os tiros (inclusive os seus!). 🛡️

- Game Feel & Tensão: A cada alien destruído, a música de fundo e a velocidade dos aliens acelera dinamicamente, aumentando a tensão psicológica da batalha! 🎵

- Power-ups Nostálgicos: Sobreviva e melhore seu arsenal com itens inspirados na cultura pop! Encontre a Fruta Estrela (*Stardew Valley*), o Cogumelo e a Maçã Dourada (*Super Mario* e *Minecraft*) para vidas extras, o Portal do End (*Minecraft*) para distorção balística e a lendária Triforce (*Zelda*) para o tiro triplo! 🌟🍄🗡️

- Boss Fight Épica: Sobreviveu ao enxame? Prepare-se para enfrentar o temido Demogorgon (*Stranger Things*). Ele se teletransporta pela tela com efeitos de glitch e invoca hordas de Demobats em movimentos de zigue-zague! 🦇🧇

- Easter Eggs: Se você conseguir salvar a galáxia e derrotar o chefe, a tela de vitória guarda uma visita super especial e carismática do *Baby Yoda*. 🏆👽

## 🎮 Como Jogar

Tá esperando o quê? Para começar:

1. Baixe a pasta **`Build`** aqui no repositório. (Você encontra o link para o download no arquivo **`Build.txt`**)
2. Descompacte o conteúdo em um local seguro da galáxia.
3. Abra o arquivo **`Spyte.exe`** que está dentro da pasta.
4. Se prepare pra muita invasão, melhor dizendo... Diversão! 🚀

## 💻 Bastidores & Engenharia

O projeto foi construído na Unity com scripts em C#. Todo o código é versionado usando Git e focado em boas práticas de Ciência da Computação:

- Arquitetura Desacoplada: Uso extensivo do padrão *Singleton* para gerenciadores globais, mantendo o código limpo.

- Otimização Matemática: Sistema de *Bounding Box* calculada dinamicamente em tempo real para o grupo de aliens, poupando processamento físico de colisores.

- Uma pitada de Turing & Trigonometria: Máquina de estados finita para as fases do Demogorgon e uso da função *Mathf.Sin* (onda senoidal) contínua para o voo dos Demobats.

- Prevenção de Falhas (*Edge Cases*): Blindagem contra violação de limites de vetor (*Out-of-Bounds*), tratamento rigoroso de rotinas assíncronas (*Coroutines*) nas transições de fases e bloqueio contra *softlocks* de inputs, condição de corrida em danos simultâneos (*Race Condition*), prevenção de colisão múltipla irregular (*Double Hit*) e prevenção de vazamento de memória (*Memory Leak*).

## 📚 Materiais Extras

Além do código, você pode dar uma espiadinha no *Guia Contra Invasões Alienígenas*, que é um **PDF com todos os detalhes e um guia ilustrado** (*GDD - Game Design Document*) no repositório para você conferir e entender tudinho. 📄✨

## 📬 Fale Comigo

Quer trocar ideia, sugerir algo ou só dar um alô? Me chama lá no Instagram:  
👩‍💻 [@neblina.th](https://www.instagram.com/neblina.th/)

Divirta-se e bora salvar a galáxia! 🌌👾🔥

**Que a Força esteja com você! 💫**
