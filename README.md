<h1>Tiss</h1>
<p>Tiss is a top-down survival/horde game. Build your base and defend against waves of zombies in an ever-changing maze.</p>
<p>Version 1.1 - Iteration 3</p>
<img src="/Resources/TissShowcase.png" alt="Yep" title="Fjert">

<h2>How to play</h2>
<p>You can play Tiss online at https://torbst.itch.io/tiss</p>
<h3>Previous releases</h3>
<p>Previous releases are available here at GitHub (Windows only).</p>
<ul>
<li>Navigate to Releases and download the .zip file (not source code!)</li>
<li>Unzip</li>
<li>Run the .exe file</li>
</ul>

<h2>About the project (nerd shit)</h2>
During this project I learned a lot about Unity's coroutines, and the impact when used on an architectural scale.
I found that this usage of coroutines increases performance, but reduces modifiability.
By spreading computations over several frames, it's easy to avoid lag spikes. Such lag spikes are very noticeable and considerably reduce the user experience.
However, since coroutines take time, objects can't immediately expect an answer when they instruct an external object to start a coroutine.
For this reason, I've played around with the observer pattern. LoaderSystem, PortalSystem, PathfindingSystem and many more use this.
All of the coroutine-using systems had their own listener interface, but once they began to pile up, I decided to instead implement a custom EventSystem.
Any object implementing the IEventListener interface can listen to any events, and any System can declare an event.
My implementation of the EventSystem doesn't provide information about which system that declared an event, but I don't think this is a huge disadvantage.
Systems use the singleton pattern, so they're easily accessible anyway.
