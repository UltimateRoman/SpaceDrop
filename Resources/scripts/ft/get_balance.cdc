import FungibleToken from "../../contracts/utility/FungibleToken.cdc"
import SampleToken from "../../contracts/SampleToken.cdc"

pub fun main(account: Address): UFix64 {
    let acct = getAccount(account)
    let vaultRef = acct.getCapability(SampleToken.VaultPublicPath)
        .borrow<&SampleToken.Vault{FungibleToken.Balance}>()
        ?? panic("Could not borrow Balance reference to the Vault")

    return vaultRef.balance
}